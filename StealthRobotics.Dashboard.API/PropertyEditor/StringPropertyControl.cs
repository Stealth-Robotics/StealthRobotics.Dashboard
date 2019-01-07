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
            string startingVal = (string)prop.GetValue(source);
            Value = startingVal;
            TextBox b = new TextBox();
            b.TextChanged += B_TextChanged;
            b.Text = startingVal;
            layoutRoot.Children.Add(b);
        }

        private void B_TextChanged(object sender, TextChangedEventArgs e)
        {
            Value = (sender as TextBox).Text;
        }
    }
}
