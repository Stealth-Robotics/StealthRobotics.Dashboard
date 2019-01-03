using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StealthRobotics.Dashboard.API.PropertyEditor
{
    /// <summary>
    /// Interaction logic for PropertyDialog.xaml
    /// </summary>
    public partial class PropertyDialog : Window
    {
        List<PropertyControl> properties = new List<PropertyControl>();

        public PropertyDialog()
        {
            InitializeComponent();
        }

        public PropertyDialog(object propertySource)
        {
            InitializeComponent();

            Type t = propertySource.GetType();
            IEnumerable<PropertyInfo> props = t.GetProperties().Where((x) =>
            {
                IEnumerable<Attribute> attrs = x.GetCustomAttributes<DialogPropertyAttribute>();
                return attrs.Count() > 0;
            });

            foreach(PropertyInfo property in props)
            {
                //add these to the display to be modified
                //based on property type, add the appropriate control (todo: add more)
                Type propType = property.PropertyType;
                PropertyControl propEditor = null;
                if(propType == typeof(string))
                {
                    propEditor = new StringPropertyControl(propertySource, property);
                }
                else if(propType == typeof(int))
                {
                    propEditor = new NumberPropertyControl(propertySource, property, 0);
                }
                else if(propType == typeof(float))
                {
                    propEditor = new NumberPropertyControl(propertySource, property, 2);
                }
                else if(propType == typeof(double))
                {
                    propEditor = new NumberPropertyControl(propertySource, property, 4);
                }
                else if(propType.IsEnum)
                {
                    propEditor = new EnumPropertyControl(propertySource, property);
                }
                else
                {
                    continue;
                }
                propEditor.Label = Util.ToTitleCase(property.Name);
                propertyDisplay.Children.Add(propEditor);
                properties.Add(propEditor);
            }
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            //apply settings
            Apply_Click(sender, e);
            DialogResult = true;
            Close();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            foreach(PropertyControl propControl in properties)
            {
                propControl.UpdateValue();
            }
        }
    }
}
