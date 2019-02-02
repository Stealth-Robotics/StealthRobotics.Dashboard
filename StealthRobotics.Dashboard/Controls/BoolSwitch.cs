using CSharpControls.Wpf;
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

namespace StealthRobotics.Dashboard.Controls
{
    [NetworkSourceListener(typeof(bool))]
    public class BoolSwitch : SourcedControl
    {
        [DialogProperty]
        public SolidColorBrush CheckedColor
        {
            get { return (SolidColorBrush)GetValue(CheckedColorProperty); }
            set { SetValue(CheckedColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CheckedColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CheckedColorProperty =
            DependencyProperty.Register("CheckedColor", typeof(SolidColorBrush), typeof(BoolSwitch), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        [DialogProperty]
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(BoolSwitch), new PropertyMetadata(null));

        //ToggleSwitch based on https://www.codeproject.com/Articles/1215232/WPF-ToggleSwitch-Control
        readonly ToggleSwitch s;

        public BoolSwitch()
        {
            s = new ToggleSwitch()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                SwitchHorizontalAlignment = HorizontalAlignment.Center,
                HeaderContentPlacement = Dock.Top,
                CheckedText = "",
                UncheckedText = "",
                SwitchPadding = new Thickness(0),
                BorderThickness = new Thickness(2),
                FontSize = 11.5,
                UncheckedBorderBrush = new SolidColorBrush(Colors.Black)
            };
            Content = s;
            //100x50
            TileGrid.SetColumnSpan(this, 2);
            TileGrid.SetRowSpan(this, 1);

            //bind some properties
            s.SetBinding(ToggleSwitch.CheckedBackgroundProperty, s, "CheckedBorderBrush");
            s.SetBinding(ToggleSwitch.CheckedBorderBrushProperty, this, "CheckedColor");
            s.SetBinding(ToggleSwitch.ContentProperty, this, "Text");

            SourceChanged += BoolSwitch_SourceChanged;
        }

        private void BoolSwitch_SourceChanged(object sender, NetworkSourceChangedEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(e.NewSource))
            {
                NetworkBinding.Update(s, ToggleSwitch.IsCheckedProperty, e.NewSource);
            }
            else
            {
                NetworkBinding.Delete(s, ToggleSwitch.IsCheckedProperty);
            }
        }
    }
}
