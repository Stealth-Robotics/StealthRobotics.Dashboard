using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.CompilerServices;

namespace StealthRobotics.Dashboard.API.Network
{
    internal class DependencyNotifyListener : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private class DependencyBinding
        {
            public DependencyPropertyDescriptor desc;
            public bool IsBound;
            public DependencyBinding(bool isBound, DependencyPropertyDescriptor d)
            {
                IsBound = isBound;
                desc = d;
            }
        }

        public readonly DependencyObject source;
        private readonly Dictionary<DependencyProperty, DependencyBinding> bindings;

        public DependencyNotifyListener(DependencyObject obj)
        {
            source = obj ?? throw new ArgumentNullException("obj", "Can't listen to nothing...");
            bindings = new Dictionary<DependencyProperty, DependencyBinding>();
        }

        //knowing these will be dictionary keys, need to override equality checking
        public override bool Equals(object obj)
        {
            return !(obj is DependencyNotifyListener other) || other.source == source;
        }

        //knowing these will be dictionary keys, need to override has function
        public override int GetHashCode()
        {
            return source.GetHashCode();
        }

        /// <summary>
        /// Begins listening to changes in the source object's dependency property
        /// </summary>
        /// <param name="dp">The property to observe</param>
        public void BindProperty(DependencyProperty dp)
        {
            DependencyPropertyDescriptor desc;
            if (!bindings.ContainsKey(dp))
            {
                desc = DependencyPropertyDescriptor.FromProperty(dp, source.GetType());
                //force that the dependency property belongs to the type of the object
                if (desc.IsAttached)//!source.GetType().IsAssignableFrom(dp.OwnerType))
                    throw new ArgumentException("Invalid property");
                //initially unbound
                bindings[dp] = new DependencyBinding(false, desc);
            }
            else
            {
                desc = bindings[dp].desc;
            }
            //refresh the value listener
            if (!bindings[dp].IsBound)
            {
                desc.AddValueChanged(source, dpChanged);
                bindings[dp].IsBound = true;
            }
        }

        /// <summary>
        /// Temporarily stops listening to changes in the source object's dependency property
        /// </summary>
        /// <param name="dp">The property to stop observing</param>
        public void UnbindProperty(DependencyProperty dp)
        {
            if(bindings.ContainsKey(dp))
            {
                bindings[dp].IsBound = false;
                bindings[dp].desc.RemoveValueChanged(source, dpChanged);
            }
        }

        /// <summary>
        /// Begins listening to any of the source object's dependency properties that are not currently being listened to
        /// </summary>
        public void RefreshBindings()
        {
            foreach(DependencyProperty dp in bindings.Keys)
            {
                if (!bindings[dp].IsBound)
                {
                    bindings[dp].desc.AddValueChanged(source, dpChanged);
                    bindings[dp].IsBound = true;
                }
            }
        }

        /// <summary>
        /// Temporarily stops listening to all the source object's dependency properties
        /// </summary>
        public void UnbindAll()
        {
            foreach(DependencyProperty dp in bindings.Keys)
            {
                if(bindings[dp].IsBound)
                {
                    bindings[dp].desc.RemoveValueChanged(source, dpChanged);
                }
            }
        }

        /// <summary>
        /// Permanently stops listening to the source object's dependency property
        /// </summary>
        /// <param name="dp">The property to remove</param>
        public void DeleteBinding(DependencyProperty dp)
        {
            if (bindings.ContainsKey(dp))
            {
                if (bindings[dp].IsBound)
                {
                    bindings[dp].desc.RemoveValueChanged(source, dpChanged);
                    //just to be thorough
                    bindings[dp].IsBound = false;
                }
                bindings.Remove(dp);
            }
        }

        private void dpChanged(object sender, EventArgs e)
        {
            //alert that all bound dependency properties may have changed. we can't zero in on the proper one
            foreach(DependencyProperty dp in bindings.Keys)
            {
                if(bindings[dp].IsBound)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(dp.Name));
                }
            }
        }
    }
}
