using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace StealthRobotics.Dashboard.API.PropertyEditor
{
    public class StringPropertyControl : PropertyControl
    {
        public StringPropertyControl(object source, PropertyInfo prop) : base(source, prop)
        {
            TextBox b = new TextBox();
            b.TextChanged += B_TextChanged;
            b.Text = (string)prop.GetValue(source);
            layoutRoot.Children.Add(b);
        }

        private void B_TextChanged(object sender, TextChangedEventArgs e)
        {
            Value = (sender as TextBox).Text;
        }
    }
}
