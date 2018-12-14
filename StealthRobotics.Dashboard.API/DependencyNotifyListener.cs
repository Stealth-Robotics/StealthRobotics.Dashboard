using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.CompilerServices;

namespace StealthRobotics.Dashboard.API
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

        public override bool Equals(object obj)
        {
            return !(obj is DependencyNotifyListener comp) || comp.source == source;
        }

        public override int GetHashCode()
        {
            return source.GetHashCode();
        }

        public void BindProperty(DependencyProperty dp)
        {
            DependencyPropertyDescriptor desc;
            if (!bindings.ContainsKey(dp))
            {
                desc = DependencyPropertyDescriptor.FromProperty(dp, source.GetType());
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

        public void UnbindProperty(DependencyProperty dp)
        {
            if(bindings.ContainsKey(dp))
            {
                bindings[dp].IsBound = false;
                bindings[dp].desc.RemoveValueChanged(source, dpChanged);
            }
        }

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

        private void dpChanged(object sender, EventArgs e)
        {
            //alert that all bound dependency properties may have changed. we can't zero in on the proper one
            foreach(DependencyProperty dp in bindings.Keys)
            {
                if(bindings[dp].IsBound)
                {
                    PropertyChanged?.Invoke(source, new PropertyChangedEventArgs(dp.Name);
                }
            }
        }
    }
}
