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
using System.Windows.Media;
using System.Windows.Shapes;

namespace StealthRobotics.Dashboard.SamplePlugin
{
    /// <summary>
    /// The same PowerUpField as in PowerUpField.xaml, but using only code
    /// in case you prefer not to use and/or learn XAML
    /// </summary>
    [NetworkSourceListener(typeof(string))]
    public class PowerUpFieldCodeOnly : SourcedControl
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
                if (oldVal.Length == value.Length)
                {
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (oldVal[i] != value[i])
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
        public static readonly DependencyProperty ScaleValuesProperty =
            DependencyProperty.Register("ScaleValues", typeof(double[]), typeof(PowerUpFieldCodeOnly),
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
            DependencyProperty.Register("AllianceColor", typeof(SolidColorBrush), typeof(PowerUpFieldCodeOnly),
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

        // Using a DependencyProperty as the backing store for BlueColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OpposingColorProperty =
            DependencyProperty.Register("OpposingColor", typeof(SolidColorBrush), typeof(PowerUpFieldCodeOnly),
                new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        //a data converter to help communicate with the network table
        private readonly PowerUpFieldDataConverter dataConverter = new PowerUpFieldDataConverter();

        public PowerUpFieldCodeOnly()
        {
            //setup internals
            Grid layoutRoot = new Grid();
            //3 rows, fill each
            for(int i = 0; i < 3; i++)
            {
                layoutRoot.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                ScaleTransform flipper = new ScaleTransform();
                //bind the flip to the correct position. the first scale is nearest
                Binding scaleBinding = new Binding($"ScaleValues[{2 - i}]") { Source = this };
                BindingOperations.SetBinding(flipper, ScaleTransform.ScaleXProperty, scaleBinding);
                //this segment of the field (switch/scale)
                Grid fieldPart = new Grid
                {
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    RenderTransform = flipper
                };
                //put fieldPart in the ith row
                Grid.SetRow(fieldPart, i);
                //center part
                Rectangle centerPiece = new Rectangle
                {
                    Fill = new SolidColorBrush(Colors.LightGray),
                    Margin = new Thickness(25, 40, 25, 40)
                };
                fieldPart.Children.Add(centerPiece);
                //alliance part
                Rectangle alliancePiece = new Rectangle
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = 60,
                    Height = 60,
                    Margin = new Thickness(10)
                };
                //bind the color to the alliance part
                Binding allianceColorBinding = new Binding("AllianceColor") { Source = this };
                BindingOperations.SetBinding(alliancePiece, Rectangle.FillProperty, allianceColorBinding);
                fieldPart.Children.Add(alliancePiece);
                //opposing part
                Rectangle opposingPiece = new Rectangle
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Width = 60,
                    Height = 60,
                    Margin = new Thickness(10)
                };
                //bind the color to the alliance part
                Binding opposingColorBinding = new Binding("OpposingColor") { Source = this };
                BindingOperations.SetBinding(opposingPiece, Rectangle.FillProperty, opposingColorBinding);
                fieldPart.Children.Add(opposingPiece);
                //add the field element to the layout
                layoutRoot.Children.Add(fieldPart);
            }
            //set our content
            Content = layoutRoot;
            //set the tilegrid dimensions
            //TileGrid tiles are nearly 50x50 squares. Requesting a 200x300 space
            TileGrid.SetColumnSpan(this, 4);
            TileGrid.SetRowSpan(this, 6);
            //handle source changed
            SourceChanged += PowerUpFieldCodeOnly_SourceChanged;
        }

        private void PowerUpFieldCodeOnly_SourceChanged(object sender, NetworkSourceChangedEventArgs e)
        {
            //this is what we need to do if we get a new network source
            if (string.IsNullOrWhiteSpace(e.NewSource))
            {
                //if the new source is bad, just unbind the old one
                NetworkBinding.Delete(this, PowerUpFieldCodeOnly.ScaleValuesProperty);
            }
            else
            {
                //otherwise create/update the binding using a converter
                NetworkBinding.Update(this, PowerUpFieldCodeOnly.ScaleValuesProperty, e.NewSource, dataConverter);
            }
        }
    }
}
