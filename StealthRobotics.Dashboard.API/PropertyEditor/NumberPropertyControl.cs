using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Primitives;

namespace StealthRobotics.Dashboard.API.PropertyEditor
{
    public class NumberPropertyControl : PropertyControl
    {
        public NumberPropertyControl(object source, PropertyInfo prop, uint decimalPlaces, bool unsigned = false) 
            : base(source, prop)
        {
            DoubleUpDown spinner = new DoubleUpDown
            {
                Increment = Math.Pow(0.5, decimalPlaces),
                FormatString = $"F{decimalPlaces}",
                MouseWheelActiveTrigger = MouseWheelActiveTrigger.FocusedMouseOver
            };
            if (unsigned)
                spinner.Minimum = 0;
            spinner.ValueChanged += Spinner_ValueChanged;
            spinner.Value = (double)Convert.ChangeType(prop.GetValue(source), typeof(double));
            layoutRoot.Children.Add(spinner);
        }

        private void Spinner_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            Value = Convert.ChangeType((sender as DoubleUpDown).Value, prop.PropertyType);
        }
    }
}
