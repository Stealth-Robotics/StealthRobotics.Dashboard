using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using NetworkTables;
using NetworkTables.Tables;

namespace StealthRobotics.Dashboard.API.Network
{
    /// <summary>
    /// Provides methods for binding WPF controls and ViewModels to the network table
    /// </summary>
    public static class NetworkBinding
    {
        //lookup table for bound property: stores each bound object paired with all bound properties,
        //  the values they are bound to, and any converters to convert between types.
        //  Can lookup by a changing network table value or a changing object property in ~O(1)) time
        private static readonly Dictionary<INotifyPropertyChanged, OneToOneConversionMap<string, string>> propertyLookup = 
            new Dictionary<INotifyPropertyChanged, OneToOneConversionMap<string, string>>();

        private static Dispatcher assignmentDispatch;
                
        private static void OnLocalValueChange(object sender, PropertyChangedEventArgs e)
        {
            INotifyPropertyChanged obj = sender as INotifyPropertyChanged;
            OneToOneConversionMap<string, string> map = propertyLookup[obj];
            object bindingSource = (obj is DependencyNotifyListener) ? (object)(obj as DependencyNotifyListener).source : obj;
            //first, verify that the property that changed is bound to something
            if (map.TryGetByFirst(e.PropertyName, out string networkPath))
            {
                //grab the converter and use it if needed
                IValueConverter converter = map.GetConverterByFirst(e.PropertyName);
                PropertyInfo inf = bindingSource.GetType().GetProperty(e.PropertyName);
                object value = inf.GetValue(bindingSource);
                if (converter != null)
                {
                    //in an NTConverter (required in API) the null values are never used so we don't need to set them
                    object attemptedVal = converter.Convert(value, null, null, null);
                    //in case the conversion was invalid
                    if (attemptedVal != DependencyProperty.UnsetValue) value = attemptedVal;
                }
                //apparently network table STRONGLY dislikes null values
                if (value != null)
                {
                    //write to the dashboard
                    Value data = Value.MakeValue(value);
                    NetworkUtil.SmartDashboard.PutValue(networkPath, data);
                    //the network table doesn't know any better and won't try to notify us of this change
                    //therefore, bindings won't be updated again, so we should initiate a network table update
                    //this is ok, because we only write to network table on effective value changes
                    OnNetworkTableChange(NetworkUtil.SmartDashboard, networkPath, data, NotifyFlags.NotifyUpdate);
                }
            }
        }

        private static void OnNetworkTableChange(ITable table, string key, Value v, NotifyFlags flags)
        {
            //multiple objects could be bound to this key
            foreach(INotifyPropertyChanged source in propertyLookup.Keys)
            {
                OneToOneConversionMap<string, string> conversionMap = propertyLookup[source];
                object bindingSource = (source is DependencyNotifyListener) ? (object)(source as DependencyNotifyListener).source : source;
                if (conversionMap.TryGetBySecond(key, out string property))
                {
                    //the property that changed is bound to this object
                    //grab the converter and use it if needed
                    IValueConverter converter = conversionMap.GetConverterByFirst(property);
                    PropertyInfo inf = bindingSource.GetType().GetProperty(property);
                    object value = NetworkUtil.ReadValue(v);
                    if(converter != null)
                    {
                        //in an NTConverter (required in API) the null values are never used so we don't need to set them
                        object attemptedVal = converter.ConvertBack(value, null, null, null);
                        //in case the conversion was invalid
                        if (attemptedVal != DependencyProperty.UnsetValue) value = attemptedVal;
                    }
                    //correct any type inconsistencies (eg if we want to display an integer from the network, which only stores doubles)
                    if(value != null && value.GetType() != inf.PropertyType)
                    {
                        Type targetType = inf.PropertyType;
                        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            targetType = targetType.GetGenericArguments()[0];
                        }
                        //anything still here can make an invalid cast to let them know to use a converter
                        value = Convert.ChangeType(value, targetType);
                    }
                    //write to the object
                    assignmentDispatch.Invoke(() => inf.SetValue(bindingSource, value));
                }
            }
        }

        private static bool isRunning = false;

        /// <summary>
        /// Starts up the network binding engine, including network table access. Only call once during normal program execution
        /// </summary>
        /// <param name="team">The team number to use for mDNS connection</param>
        /// <param name="dispatcher">The dispatcher of the UI thread so writes can happen</param>
        public static void Initialize(int team, Dispatcher dispatcher, bool useDriverStation = true)
        {
            if (!isRunning)
            {
                assignmentDispatch = dispatcher;
                NetworkTable.SetClientMode();
                if (team == 0)
                    NetworkTable.SetIPAddress("localhost");
                else
                    NetworkTable.SetTeam(team);
                NetworkTable.SetUpdateRate(0.1);
                NetworkTable.SetDSClientEnabled(useDriverStation);
                NetworkTable.SetNetworkIdentity("C# Dashboard");
                NetworkTable.Initialize();
                foreach(INotifyPropertyChanged item in propertyLookup.Keys)
                {
                    (item as DependencyNotifyListener)?.RefreshBindings();
                }
                NetworkUtil.SmartDashboard.AddTableListener(OnNetworkTableChange, true);
                isRunning = true;
            }
        }

        /// <summary>
        /// Stops the network binding engine and network table access. Only call once during normal program execution
        /// </summary>
        public static void Shutdown()
        {
            if(isRunning)
            {
                //may dislike DS behavior, consider adding a delay
                NetworkTable.Shutdown();
                NetworkUtil.SmartDashboard.RemoveTableListener(OnNetworkTableChange);
                foreach(INotifyPropertyChanged item in propertyLookup.Keys)
                {
                    (item as DependencyNotifyListener)?.UnbindAll();
                }
                isRunning = false;
            }
        }
        
        /// <summary>
        /// Creates a binding between an observable object property and a network table entry
        /// </summary>
        /// <param name="source">The object to bind to</param>
        /// <param name="property">The case-sensitive name of the property to bind</param>
        /// <param name="networkPath">The full path of the entry in the network table</param>
        /// <param name="localOverride">Whether the local dashboard values should take precedence when the binding occurs</param>
        public static void Create(INotifyPropertyChanged source, string property, string networkPath, bool localOverride = false)
        {
            Create<object, object>(source, property, networkPath, null, localOverride);
        }

        /// <summary>
        /// Creates a binding between an observable object property and a network table entry of a different type
        /// </summary>
        /// <typeparam name="TLocal">The type of the entry in the dashboard</typeparam>
        /// <typeparam name="TNetwork">The type of the entry in the network table</typeparam>
        /// <param name="source">The object to bind to</param>
        /// <param name="property">The case-sensitive name of the property to bind</param>
        /// <param name="networkPath">The full path of the entry in the network table</param>
        /// <param name="converter">The conversion mapping between the network values and local values</param>
        /// <param name="localOverride">Whether the local dashboard values should take precedence when the binding occurs</param>
        public static void Create<TLocal, TNetwork>(INotifyPropertyChanged source, string property, string networkPath,
            NTConverter<TLocal, TNetwork> converter, bool localOverride = false)
        {
            if (!isRunning) throw new InvalidOperationException("Can only create bindings while the network table is running");
            //add these to our dictionary
            if (!propertyLookup.ContainsKey(source))
            {
                propertyLookup[source] = new OneToOneConversionMap<string, string>();
                //this is a new item being bound; have it notify us of updates
                source.PropertyChanged += OnLocalValueChange;
            }
            if (propertyLookup[source].TryAdd(property, networkPath))
            {
                //this means there were no binding conflicts
                //map a conversion if needed; null is a valid argument
                propertyLookup[source].MapConversionByFirst(property, converter);
                //make the values consistent by forcing an update.
                //first check if the dashboard has a value at all
                object data = NetworkUtil.SmartDashboard.GetValue(networkPath, null);
                if (data == null || localOverride)
                {
                    //send the local value to the network by "updating" the local value
                    PropertyChangedEventArgs e = new PropertyChangedEventArgs(property);
                    OnLocalValueChange(source, e);
                }
                else
                {
                    //pull the dashboard from the local value by "updating" the network value
                    OnNetworkTableChange(NetworkUtil.SmartDashboard, networkPath, Value.MakeValue(data), NotifyFlags.NotifyUpdate);
                }
            }
        }

        /// <summary>
        /// Creates a binding between a dependency property and a network table entry of a different type
        /// </summary>
        /// <param name="source">The object to bind to</param>
        /// <param name="property">The property to bind. Attached properties will throw an exception</param>
        /// <param name="networkPath">The full path of the entry in the network table</param>
        /// <param name="localOverride">Whether the local dashboard values should take precedence when the binding occurs</param>
        public static void Create(DependencyObject source, DependencyProperty property, string networkPath, bool localOverride = false)
        {
            Create<object, object>(source, property, networkPath, null, localOverride);
        }

        /// <summary>
        /// Creates a binding between a dependency property and a network table entry
        /// </summary>
        /// <typeparam name="TLocal">The type of the entry in the dashboard</typeparam>
        /// <typeparam name="TNetwork">The type of the entry in the network table</typeparam>
        /// <param name="source">The object to bind to</param>
        /// <param name="property">The property to bind. Attached properties will throw an exception</param>
        /// <param name="networkPath">The full path of the entry in the network table</param>
        /// <param name="converter">The conversion mapping between the network values and local values</param>
        /// <param name="localOverride">Whether the local dashboard values should take precedence when the binding occurs</param>
        public static void Create<TLocal, TNetwork>(DependencyObject source, DependencyProperty property, string networkPath,
            NTConverter<TLocal, TNetwork> converter, bool localOverride = false)
        {
            if (!isRunning) throw new InvalidOperationException("Can only create bindings while the network table is running");
            //because of additional work that needs to be done to bind the value, simpler to reimplement
            DependencyNotifyListener listener = new DependencyNotifyListener(source);
            if (!propertyLookup.ContainsKey(listener))
            {
                propertyLookup[listener] = new OneToOneConversionMap<string, string>();
                //this is a new item being bound, have it notify us of updates
                listener.PropertyChanged += OnLocalValueChange;
            }
            if (propertyLookup[listener].TryAdd(property.Name, networkPath))
            {
                //this means there were no binding conflicts
                //bind the dependency property to be notified of changes to it
                listener.BindProperty(property);
                //map a conversion if needed; null is valid
                propertyLookup[listener].MapConversionByFirst(property.Name, converter);
                //make the values consistent by forcing an update.
                //first check if the dashboard has a value at all
                Value data = NetworkUtil.SmartDashboard.GetValue(networkPath, null);
                if (data == null || localOverride)
                {
                    //send the local value to the network by "updating" the local value
                    PropertyChangedEventArgs e = new PropertyChangedEventArgs(property.Name);
                    OnLocalValueChange(listener, e);
                }
                else
                {
                    //pull the dashboard from the local value by "updating" the network value
                    OnNetworkTableChange(NetworkUtil.SmartDashboard, networkPath, data, NotifyFlags.NotifyUpdate);
                }
            }
        }

        /// <summary>
        /// Deletes a binding between an observable object property and a network table entry
        /// </summary>
        /// <param name="source">The object the binding is on</param>
        /// <param name="property">The property to unbind</param>
        public static void Delete(INotifyPropertyChanged source, string property)
        {
            if (!isRunning) throw new InvalidOperationException("Can only delete bindings while the network table is running");
            if(propertyLookup.ContainsKey(source))
            {
                propertyLookup[source].TryRemoveByFirst(property);
            }
        }

        /// <summary>
        /// Deletes a binding between a dependency property and a network table entry
        /// </summary>
        /// <param name="source">The object the binding is on</param>
        /// <param name="property">The property to unbind</param>
        public static void Delete(DependencyObject source, DependencyProperty property)
        {
            if (!isRunning) throw new InvalidOperationException("Can only delete bindings while the network table is running");
            DependencyNotifyListener listener = new DependencyNotifyListener(source);
            if (propertyLookup.ContainsKey(listener))
            {
                if(propertyLookup[listener].TryRemoveByFirst(property.Name))
                {
                    //permanent delete rather than temporary unbind
                    listener.DeleteBinding(property);
                }
            }
        }

        /// <summary>
        /// Updates an existing binding between an observable object property and a network table entry
        /// </summary>
        /// <param name="source">The object the binding is on</param>
        /// <param name="property">The new property to bind</param>
        /// <param name="networkPath">The full path of the new entry in the network table</param>
        /// <param name="localOverride">Whether the local dashboard values should take precedence when the binding occurs</param>
        public static void Update(INotifyPropertyChanged source, string property, string networkPath, bool localOverride = false)
        {
            Update<object, object>(source, property, networkPath, null, localOverride);
        }

        /// <summary>
        /// Updates an existing binding between an observable object property and a network table entry of a different type
        /// </summary>
        /// <typeparam name="TLocal">The type of the entry in the dashboard</typeparam>
        /// <typeparam name="TNetwork">The type of the entry in the network table</typeparam>
        /// <param name="source">The object the binding is on</param>
        /// <param name="property">The new property to bind</param>
        /// <param name="networkPath">The full path of the new entry in the network table</param>
        /// <param name="converter">The conversion mapping between network values and local values</param>
        /// <param name="localOverride">Whether the local dashboard values should take precendence when the binding occurs</param>
        public static void Update<TLocal, TNetwork>(INotifyPropertyChanged source, string property, string networkPath,
            NTConverter<TLocal, TNetwork> converter, bool localOverride = false)
        {
            Delete(source, property);
            Create(source, property, networkPath, converter, localOverride);
        }

        /// <summary>
        /// Updates an existing binding between a dependency property and a network table entry
        /// </summary>
        /// <param name="source">The object to bind to</param>
        /// <param name="property">The new property to bind. Attached properties will throw an exception</param>
        /// <param name="networkPath">The full path of the new entry in the network table</param>
        /// <param name="localOverride">Whether the local dashboard values should take precendence when the binding occurs</param>
        public static void Update(DependencyObject source, DependencyProperty property, string networkPath, bool localOverride = false)
        {
            Update<object, object>(source, property, networkPath, null, localOverride);
        }

        /// <summary>
        /// Updates an existing binding between a dependency property and a network table entry
        /// </summary>
        /// <typeparam name="TLocal">The type of the entry in the dashboard</typeparam>
        /// <typeparam name="TNetwork">The type of the entry in the network table</typeparam>
        /// <param name="source">The object to bind to</param>
        /// <param name="property">The new property to bind. Attached properties will throw an exception</param>
        /// <param name="networkPath">The full path of the new entry in the network table</param>
        /// <param name="converter">The conversion mapping between network values and local values</param>
        /// <param name="localOverride">Whether the local dashboard values should take precendence when the binding occurs</param>
        public static void Update<TLocal, TNetwork>(DependencyObject source, DependencyProperty property, string networkPath,
            NTConverter<TLocal, TNetwork> converter, bool localOverride = false)
        {
            Delete(source, property);
            Create(source, property, networkPath, converter, localOverride);
        }
    }
}
