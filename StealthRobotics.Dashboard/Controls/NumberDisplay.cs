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

namespace StealthRobotics.Dashboard.Controls
{
    [NetworkSourceListener(typeof(double))]
    public class NumberDisplay : SourcedControl
    {
        [DialogProperty]
        public new int FontSize
        {
            get { return (int)(double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, (double)value); }
        }

        [DialogProperty]
        public NumberFormat Format
        {
            get { return (NumberFormat)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Format.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format", typeof(NumberFormat), typeof(NumberDisplay), new PropertyMetadata(NumberFormat.General, OnFormatChanged));

        private static void OnFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberDisplay x = (d as NumberDisplay);
            x.converter.Format = (NumberFormat)e.NewValue;
            //indicate that we should force a network update so that the formatting change can propagate
            x.NumberDisplay_SourceChanged(x, new NetworkSourceChangedEventArgs(x.Source, x.Source));
        }

        [DialogProperty]
        public int Precision
        {
            get { return (int)GetValue(PrecisionProperty); }
            set { SetValue(PrecisionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Precision.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrecisionProperty =
            DependencyProperty.Register("Precision", typeof(int), typeof(NumberDisplay), new PropertyMetadata(3, OnPrecisionChanged));

        private static void OnPrecisionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberDisplay x = (d as NumberDisplay);
            x.converter.Precision = (int)e.NewValue;
            //indicate that we should force a network update to make the formatting propagate
            x.NumberDisplay_SourceChanged(x, new NetworkSourceChangedEventArgs(x.Source, x.Source));
        }

        readonly TextBlock display;
        readonly NumberFormatConverter converter;

        public NumberDisplay()
        {
            converter = new NumberFormatConverter() { Precision = this.Precision, Format = this.Format };
            display = new TextBlock()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Content = display;
            //100x50
            TileGrid.SetColumnSpan(this, 2);
            TileGrid.SetRowSpan(this, 1);

            //REMOVE THIS LATER
            display.Text = "test";
            display.SetBinding(TextBlock.FontSizeProperty, this, "FontSize");
            SourceChanged += NumberDisplay_SourceChanged;
        }

        private void NumberDisplay_SourceChanged(object sender, NetworkSourceChangedEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(e.NewSource))
            {
                NetworkBinding.Update(display, TextBlock.TextProperty, e.NewSource, converter);
            }
            else
            {
                NetworkBinding.Delete(display, TextBlock.TextProperty);
            }
        }
    }
}
