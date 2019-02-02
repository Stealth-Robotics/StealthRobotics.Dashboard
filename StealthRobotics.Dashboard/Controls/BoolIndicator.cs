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
using System.Windows.Media;
using System.Windows.Shapes;

namespace StealthRobotics.Dashboard.Controls
{
    [NetworkSourceListener(typeof(bool))]
    public class BoolIndicator : SourcedControl
    {
        [DialogProperty]
        public SolidColorBrush OnColor
        {
            get { return (SolidColorBrush)GetValue(OnColorProperty); }
            set { SetValue(OnColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OnColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OnColorProperty =
            DependencyProperty.Register("OnColor", typeof(SolidColorBrush), typeof(BoolIndicator), new PropertyMetadata(new SolidColorBrush(Colors.Green)));

        [DialogProperty]
        public SolidColorBrush OffColor
        {
            get { return (SolidColorBrush)GetValue(OffColorProperty); }
            set { SetValue(OffColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OffColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffColorProperty =
            DependencyProperty.Register("OffColor", typeof(SolidColorBrush), typeof(BoolIndicator), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public SolidColorBrush CurrentBrush
        {
            get { return (SolidColorBrush)GetValue(CurrentBrushProperty); }
            set { SetValue(CurrentBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentBrushProperty =
            DependencyProperty.Register("CurrentBrush", typeof(SolidColorBrush), typeof(BoolIndicator), new PropertyMetadata(null));

        public bool Value
        {
            get { return (bool)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(bool), typeof(BoolIndicator), new PropertyMetadata(false, ValueChanged));

        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as BoolIndicator).Paint();
        }

        public BoolIndicator()
        {
            Paint();
            Ellipse el = new Ellipse()
            {
                Width = 25,
                Height = 25,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Content = el;
            //50x50 is default size, nothing to do here

            el.SetBinding(Ellipse.StrokeProperty, this, "CurrentBrush");
            el.SetBinding(Ellipse.FillProperty, this, "CurrentBrush");
            SourceChanged += BoolIndicator_SourceChanged;
        }

        private void Paint()
        {
            this.SetBinding(CurrentBrushProperty, this, Value ? "OnColor" : "OffColor");
        }

        private void BoolIndicator_SourceChanged(object sender, NetworkSourceChangedEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(e.NewSource))
            {
                NetworkBinding.Update(this, ValueProperty, e.NewSource);
            }
            else
            {
                NetworkBinding.Delete(this, ValueProperty);
            }
        }
    }
}
