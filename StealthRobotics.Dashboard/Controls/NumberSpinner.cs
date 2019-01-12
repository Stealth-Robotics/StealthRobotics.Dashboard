using StealthRobotics.Dashboard.API.Network;
using StealthRobotics.Dashboard.API.PropertyEditor;
using StealthRobotics.Dashboard.API.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xceed.Wpf.Toolkit;

namespace StealthRobotics.Dashboard.Controls
{
    [NetworkSourceListener(typeof(double))]
    public class NumberSpinner : SourcedControl
    {
        [DialogProperty]
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(NumberSpinner), new PropertyMetadata(double.NegativeInfinity));

        [DialogProperty]
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(NumberSpinner), new PropertyMetadata(double.PositiveInfinity));

        [DialogProperty]
        public double Interval
        {
            get { return (double)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Interval.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(double), typeof(NumberSpinner), new PropertyMetadata(0.1));

        [DialogProperty]
        public NumberFormat Format
        {
            get { return (NumberFormat)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Format.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format", typeof(NumberFormat), typeof(NumberSpinner), 
                new PropertyMetadata(NumberFormat.General, OnDisplayFormatChanged));

        [DialogProperty]
        public int Precision
        {
            get { return (int)GetValue(PrecisionProperty); }
            set { SetValue(PrecisionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Precision.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrecisionProperty =
            DependencyProperty.Register("Precision", typeof(int), typeof(NumberSpinner), 
                new PropertyMetadata(3, OnDisplayFormatChanged));

        private static void OnDisplayFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberSpinner sender = d as NumberSpinner;
            sender.spinner.FormatString = $"{(char)sender.Format}{sender.Precision}";
        }

        private DoubleUpDown spinner;

        public NumberSpinner()
        {
            spinner = new DoubleUpDown()
            {
                Margin = new Thickness(2),
                VerticalAlignment = VerticalAlignment.Center,
                Value = 0
            };
            Content = spinner;
            //150x50 should be fine
            TileGrid.SetColumnSpan(this, 3);
            TileGrid.SetRowSpan(this, 1);

            //bind editor properties
            spinner.SetBinding(DoubleUpDown.MinimumProperty, this, "Minimum");
            spinner.SetBinding(DoubleUpDown.MaximumProperty, this, "Maximum");
            spinner.SetBinding(DoubleUpDown.IncrementProperty, this, "Interval");
            SourceChanged += SourcedSpinner_SourceChanged;
        }

        private void SourcedSpinner_SourceChanged(object sender, NetworkSourceChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.NewSource))
            {
                NetworkBinding.Update(spinner, DoubleUpDown.ValueProperty, e.NewSource);
            }
            else
            {
                NetworkBinding.Delete(spinner, DoubleUpDown.ValueProperty);
            }
        }
    }
}
