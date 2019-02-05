using StealthRobotics.Dashboard.API.UI;
using StealthRobotics.Dashboard.API.Network;
using StealthRobotics.Dashboard.API.PropertyEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace StealthRobotics.Dashboard.Controls
{
    [ComplexNetworkListener("SendableChooser")]
    public class Selector : SourcedControl, INotifyPropertyChanged
    {
        string[] opts = new string[0];
        public string[] Options
        {
            set
            {
                if(!Util.ArraysEqual(opts, value))
                {
                    opts = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Options"));
                }
            }
            get
            {
                //return a copy so that there's no undue messing around
                return opts?.ToArray();
            }
        }

        string def = "";
        public string Default
        {
            set
            {
                if(value != def)
                {
                    def = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Default"));
                }
            }
            get
            {
                return def;
            }
        }

        readonly ComboBox options;

        public event PropertyChangedEventHandler PropertyChanged;

        public Selector()
        {
            options = new ComboBox()
            {
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(2)
            };
            Content = options;

            TileGrid.SetColumnSpan(this, 3);
            TileGrid.SetRowSpan(this, 1);

            PropertyChanged += SendableChooserControl_PropertyChanged;
            SourceChanged += SendableChooserControl_SourceChanged;
        }

        private void SendableChooserControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object savedSelection = options.SelectedItem;
            options.Items.Clear();
            if (Options != null)
            {
                foreach (string option in Options)
                {
                    options.Items.Add(option);
                }
            }
            if (e.PropertyName == "Default")
            {
                options.SelectedItem = Default;
            }
            else options.SelectedItem = savedSelection;
        }

        private void SendableChooserControl_SourceChanged(object sender, NetworkSourceChangedEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(e.NewSource))
            {
                NetworkBinding.Update(this, "Options", e.NewSource + "/options");
                NetworkBinding.Update(this, "Default", e.NewSource + "/default");
                NetworkBinding.Update(options, ComboBox.SelectedItemProperty, e.NewSource + "/selected");
            }
            else
            {
                NetworkBinding.Delete(this, "Options");
                NetworkBinding.Delete(this, "Default");
                NetworkBinding.Delete(options, ComboBox.SelectedItemProperty);
            }
        }

        private void DoListRefresh()
        {
            options.Items.Clear();
            foreach(string option in Options)
            {
                options.Items.Add(option);
            }
            options.SelectedItem = Default;
        }
    }
}
