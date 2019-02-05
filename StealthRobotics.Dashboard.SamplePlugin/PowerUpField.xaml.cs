using StealthRobotics.Dashboard.API.Network;
using StealthRobotics.Dashboard.API.PropertyEditor;
using StealthRobotics.Dashboard.API.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StealthRobotics.Dashboard.SamplePlugin
{
    /// <summary>
    /// Interaction logic for PowerUpField.xaml
    /// A simple example plugin that takes a string and uses a custom converter
    /// to display the state of a 2018 FIRST Power Up field
    /// </summary>
    [NetworkSourceListener(typeof(string))]
    public partial class PowerUpField : SourcedControl
    {
        /// <summary>
        /// An array that tracks which segments should be flipped. Generally, use caution when binding arrays to dashboard
        /// and to controls. If you want to bind data as an array to the dashboard, you need to create a new array.
        /// Changing the contents of the array does not cause the binding to trigger an update.
        /// In this case, we only need to read from the dashboard so this is fine
        /// </summary>
        public double[] ScaleValues
        {
            get { return (double[])GetValue(ScaleValuesProperty); }
            set
            {
                //we need to check for array equality so we don't create infinite updates
                double[] oldVal = (double[])GetValue(ScaleValuesProperty);
                if(oldVal.Length == value.Length)
                {
                    for(int i = 0; i < value.Length; i++)
                    {
                        if(oldVal[i] != value[i])
                        {
                            //discrepancy
                            SetValue(ScaleValuesProperty, value);
                            return;
                        }
                    }
                    //no discrepancy
                }
                else
                {
                    SetValue(ScaleValuesProperty, value);
                }
            }
        }

        // Using a DependencyProperty as the backing store for ScaleValues.  This enables animation, styling, binding, etc...
        // default is { 1, -1, 1 } - middle one flipped for previewing
        public static readonly DependencyProperty ScaleValuesProperty =
            DependencyProperty.Register("ScaleValues", typeof(double[]), typeof(PowerUpField), 
                new PropertyMetadata(new double[] { 1, -1, 1 }));

        /// <summary>
        /// The color that should display on our alliance side of the elements.
        /// Note the usage of DialogPropertyAttribute to show that
        /// this can be edited in the property editor
        /// </summary>
        [DialogProperty]
        public SolidColorBrush AllianceColor
        {
            get { return (SolidColorBrush)GetValue(AllianceColorProperty); }
            set { SetValue(AllianceColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllianceColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllianceColorProperty =
            DependencyProperty.Register("AllianceColor", typeof(SolidColorBrush), typeof(PowerUpField),
                new PropertyMetadata(new SolidColorBrush(Colors.Purple)));

        /// <summary>
        /// The color that should display on the opposing alliance side of the elements.
        /// The property editor currently supports editing strings, numeric types (int, float, double),
        /// enums, Color, and SolidColorBrush
        /// </summary>
        [DialogProperty]
        public SolidColorBrush OpposingColor
        {
            get { return (SolidColorBrush)GetValue(OpposingColorProperty); }
            set { SetValue(OpposingColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OpposingColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OpposingColorProperty =
            DependencyProperty.Register("OpposingColor", typeof(SolidColorBrush), typeof(PowerUpField),
                new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        //a data converter to help communicate with the network table
        private readonly PowerUpFieldDataConverter dataConverter = new PowerUpFieldDataConverter();

        public PowerUpField()
        {
            InitializeComponent();
            //In a constructor, we must always request a TileGrid size and handle source updates
            //TileGrid tiles are nearly 50x50 squares. Requesting a 200x300 space
            TileGrid.SetColumnSpan(this, 4);
            TileGrid.SetRowSpan(this, 6);

            SourceChanged += PowerUpField_SourceChanged;
        }

        private void PowerUpField_SourceChanged(object sender, NetworkSourceChangedEventArgs e)
        {
            //To handle a source change, we should do a few things.
            //First, we need to check if the new source is valid or not
            if(string.IsNullOrWhiteSpace(e.NewSource))
            {
                //if the new source is bad, just unbind the old one
                NetworkBinding.Delete(this, PowerUpField.ScaleValuesProperty);
            }
            else
            {
                //otherwise create/update the binding using a converter
                NetworkBinding.Update(this, PowerUpField.ScaleValuesProperty, e.NewSource, dataConverter);
            }
        }
    }
}
