using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace StealthRobotics.Dashboard.API.PropertyEditor
{
    public class ColorPropertyControl : PropertyControl
    {
        readonly bool isBrush;

        public ColorPropertyControl(object source, PropertyInfo prop) : base(source, prop)
        {
            isBrush = prop.PropertyType == typeof(SolidColorBrush);
            object startingVal = prop.GetValue(source);

            Value = startingVal;
            ColorCanvas selector = new ColorCanvas() { MaxWidth = 250, HorizontalAlignment = HorizontalAlignment.Left };
            selector.SelectedColorChanged += Selector_SelectedColorChanged;
            selector.SelectedColor = GetColor(startingVal);
            layoutRoot.Children.Add(selector);
        }

        private void Selector_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Value = GetValue((sender as ColorCanvas).SelectedColor ?? Colors.Transparent);
        }

        private Color GetColor(object val)
        {
            if (isBrush)
                return (val as SolidColorBrush)?.Color ?? Colors.Transparent;
            return (Color)val;
        }

        private object GetValue(Color val)
        {
            if (isBrush)
                return new SolidColorBrush(val);
            return val;
        }
    }
}
