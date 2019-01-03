using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace StealthRobotics.Dashboard.API.PropertyEditor
{
    public class EnumPropertyControl : PropertyControl
    {
        public EnumPropertyControl(object source, PropertyInfo prop) 
            : base(source, prop)
        {
            IEnumerable<string> allNames = Enum.GetNames(prop.PropertyType).Select(x => Util.ToTitleCase(x));
            string curName = Util.ToTitleCase(Enum.GetName(prop.PropertyType, prop.GetValue(source)));

            ComboBox list = new ComboBox();
            foreach(string s in allNames)
            {
                list.Items.Add(s);
            }
            list.SelectionChanged += List_SelectionChanged;
            list.SelectedItem = curName;

            layoutRoot.Children.Add(list);
        }

        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Value = Enum.Parse(prop.PropertyType, Util.ToCamelCase((string)(sender as ComboBox).SelectedItem));
        }
    }
}
