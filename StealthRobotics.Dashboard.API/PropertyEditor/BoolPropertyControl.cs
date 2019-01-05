using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace StealthRobotics.Dashboard.API.PropertyEditor
{
    public class BoolPropertyControl : PropertyControl
    {
        public BoolPropertyControl(object source, PropertyInfo prop) : base(source, prop)
        {
            //do some minimal rearranging
            StackPanel p = new StackPanel() { Orientation = Orientation.Horizontal };
            layoutRoot.Children.Add(p);
            
            layoutRoot.Children.Remove(label);
            p.Children.Add(label);

            CheckBox b = new CheckBox() { Margin = new Thickness(2, 0, 0, 0) };
            b.Checked += CheckBoxToggle;
            b.Unchecked += CheckBoxToggle;
            b.IsChecked = (bool)prop.GetValue(source);
            p.Children.Add(b);
        }

        private void CheckBoxToggle(object sender, RoutedEventArgs e)
        {
            Value = (sender as CheckBox).IsChecked ?? false;
        }
    }
}
