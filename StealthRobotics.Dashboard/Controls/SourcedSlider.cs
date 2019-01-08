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
    public class SourcedSlider : SourcedControl
    {
        [DialogProperty]
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(SourcedSlider), new PropertyMetadata(-10.0));

        [DialogProperty]
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(SourcedSlider), new PropertyMetadata(10.0));

        [DialogProperty]
        public double Interval
        {
            get { return (double)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Interval.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(double), typeof(SourcedSlider), new PropertyMetadata(0.01));

        [DialogProperty]
        public bool SnapToInterval
        {
            get { return (bool)GetValue(SnapToIntervalProperty); }
            set { SetValue(SnapToIntervalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SnapToInterval.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SnapToIntervalProperty =
            DependencyProperty.Register("SnapToInterval", typeof(bool), typeof(SourcedSlider), new PropertyMetadata(false));

        [DialogProperty]
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SourcedSlider),
                new PropertyMetadata(Orientation.Horizontal, OrientationChanged));

        private static void OrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            int oldRowSpan = TileGrid.GetRowSpan(sender);
            int oldColSpan = TileGrid.GetColumnSpan(sender);
            TileGrid.SetColumnSpan(sender, oldRowSpan);
            TileGrid.SetRowSpan(sender, oldColSpan);
            HorizontalAlignment oldHAlign = (sender as SourcedSlider).s.HorizontalAlignment;
            VerticalAlignment oldVAlign = (sender as SourcedSlider).s.VerticalAlignment;
            (sender as SourcedSlider).s.HorizontalAlignment = 
                oldHAlign == HorizontalAlignment.Center ? HorizontalAlignment.Stretch : HorizontalAlignment.Center;
            (sender as SourcedSlider).s.VerticalAlignment =
                oldVAlign == VerticalAlignment.Center ? VerticalAlignment.Stretch : VerticalAlignment.Center;
        }

        private Slider s;

        public SourcedSlider() : this("") { }

        public SourcedSlider(string source)
        {
            SourceType = typeof(double);
            s = new Slider()
            {
                Margin = new Thickness(2),
                VerticalAlignment = VerticalAlignment.Center
            };
            Content = s;
            //reasonable size 150x50
            TileGrid.SetColumnSpan(this, 3);
            TileGrid.SetRowSpan(this, 1);

            //bind properties of the slider
            s.SetBinding(Slider.OrientationProperty, this, "Orientation");
            s.SetBinding(Slider.MinimumProperty, this, "Minimum");
            s.SetBinding(Slider.MaximumProperty, this, "Maximum");
            s.SetBinding(Slider.IsSnapToTickEnabledProperty, this, "SnapToInterval");
            s.SetBinding(Slider.TickFrequencyProperty, this, "Interval");
            SourceChanged += SourcedSlider_SourceChanged;
        }

        private void SourcedSlider_SourceChanged(object sender, NetworkSourceChangedEventArgs e)
        {
            NetworkBinding.Update(s, Slider.ValueProperty, e.NewSource);
        }
    }
}
