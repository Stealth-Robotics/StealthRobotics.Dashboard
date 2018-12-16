using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using NetworkTables;
using NetworkTables.Tables;

namespace StealthRobotics.Dashboard.API
{
    public static class NetworkBinding
    {
        //lookup table for bound property: stores each bound object paired with all bound properties,
        //  the values they are bound to, and any converters to convert between types.
        //  Can lookup by a changing network table value or a changing object property in ~O(1)) time
        private static readonly Dictionary<INotifyPropertyChanged, OneToOneConversionMap<string, string>> propertyLookup = 
            new Dictionary<INotifyPropertyChanged, OneToOneConversionMap<string, string>>();

        private static Dispatcher assignmentDispatch;

        /// <summary>
        /// The SmartDashboard network table path
        /// </summary>
        private static NetworkTable Dashboard
        {
            get
            {
                return NetworkTable.GetTable("/SmartDashboard");
            }
        }

        private static Type TypeOf(Value v)
        {
            switch(v?.Type)
            {
                case NtType.Boolean:
                    return typeof(bool);
                case NtType.BooleanArray:
                    return typeof(bool[]);
                case NtType.Double:
                    return typeof(double);
                case NtType.DoubleArray:
                    return typeof(double[]);
                case NtType.String:
                    return typeof(string);
                case NtType.StringArray:
                    return typeof(string[]);
                case NtType.Raw:
                    return typeof(byte[]);
                case NtType.Rpc:
                    //remote procedure call, should not be trying to handle these
                    throw new InvalidCastException("Can't handle bindings to remote procedure calls");
                default:
                    return null;
            }
        }

        /// <summary>
        /// Converts the NetworkTable simple value into an actual .NET data type with data
        /// </summary>
        /// <param name="v">The value to convert</param>
        public static object ReadValue(Value v)
        {
            switch (v?.Type)
            {
                case NtType.Boolean:
                    return v.GetBoolean();
                case NtType.BooleanArray:
                    return v.GetBooleanArray();
                case NtType.Double:
                    return v.GetDouble();
                case NtType.DoubleArray:
                    return v.GetDoubleArray();
                case NtType.String:
                    return v.GetString();
                case NtType.StringArray:
                    return v.GetStringArray();
                default:
                    return null;
            }
        }

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
                //write to the dashboard
                Value data = Value.MakeValue(value);
                Dashboard.PutValue(networkPath, data);
                //the network table doesn't know any better and won't try to notify us of this change
                //therefore, bindings won't be updated again, so we should initiate a network table update
                //this is ok, because we only write to network table on effective value changes
                OnNetworkTableChange(Dashboard, networkPath, data, NotifyFlags.NotifyUpdate);
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
                    object value = ReadValue(v);
                    if(converter != null)
                    {
                        //in an NTConverter (required in API) the null values are never used so we don't need to set them
                        object attemptedVal = converter.ConvertBack(value, null, null, null);
                        //in case the conversion was invalid
                        if (attemptedVal != DependencyProperty.UnsetValue) value = attemptedVal;
                    }
                    //correct any type inconsitencies (eg if we want to display an integer from the network, which only stores doubles)
                    if(value.GetType() != inf.PropertyType)
                    {
                        value = Convert.ChangeType(value, inf.PropertyType);
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
        public static void Initialize(int team, Dispatcher dispatcher)
        {
            if (!isRunning)
            {
                assignmentDispatch = dispatcher;
                NetworkTable.SetClientMode();
                NetworkTable.SetTeam(team);
                //for local testing
                //NetworkTable.SetIPAddress("localhost");
                NetworkTable.SetUpdateRate(0.1);
                NetworkTable.SetNetworkIdentity("C# Dashboard");
                NetworkTable.Initialize();
                foreach(INotifyPropertyChanged item in propertyLookup.Keys)
                {
                    (item as DependencyNotifyListener)?.RefreshBindings();
                }
                Dashboard.AddTableListener(OnNetworkTableChange, true);
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
                Dashboard.RemoveTableListener(OnNetworkTableChange);
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
        /// <param name="property">The case-sensitive name of the property to bind to</param>
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
        /// <param name="property">The case-sensitive name of the property to bind to</param>
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
                object data = Dashboard.GetValue(networkPath, null);
                if (data == null || localOverride)
                {
                    //send the local value to the network by "updating" the local value
                    PropertyChangedEventArgs e = new PropertyChangedEventArgs(property);
                    OnLocalValueChange(source, e);
                }
                else
                {
                    //pull the dashboard from the local value by "updating" the network value
                    OnNetworkTableChange(Dashboard, networkPath, Value.MakeValue(data), NotifyFlags.NotifyUpdate);
                }
            }
        }

        /// <summary>
        /// Creates a binding between a dependency property and a network table entry of a different type
        /// </summary>
        /// <param name="source">The object to bind to</param>
        /// <param name="property">The property to bind to. Attached properties will throw an exception</param>
        /// <param name="networkPath">The full path of the entry in the network table</param>
        /// <param name="localOverride">Whether the local dashboard values should take precedence when the binding occurs</param>
        public static void Create<TLocal, TNetwork>(DependencyObject source, DependencyProperty property, string networkPath, bool localOverride = false)
        {
            Create<object, object>(source, property, networkPath, null, localOverride);
        }

        /// <summary>
        /// Creates a binding between a dependency property and a network table entry
        /// </summary>
        /// <typeparam name="TLocal">The type of the entry in the dashboard</typeparam>
        /// <typeparam name="TNetwork">The type of the entry in the network table</typeparam>
        /// <param name="source">The object to bind to</param>
        /// <param name="property">The property to bind to. Attached properties will throw an exception</param>
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
                Value data = Dashboard.GetValue(networkPath, null);
                if (data == null || localOverride)
                {
                    //send the local value to the network by "updating" the local value
                    PropertyChangedEventArgs e = new PropertyChangedEventArgs(property.Name);
                    OnLocalValueChange(listener, e);
                }
                else
                {
                    //pull the dashboard from the local value by "updating" the network value
                    OnNetworkTableChange(Dashboard, networkPath, data, NotifyFlags.NotifyUpdate);
                }
            }
        }
    }
}
