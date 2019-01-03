using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StealthRobotics.Dashboard.API.PropertyEditor
{
    /// <summary>
    /// Interaction logic for PropertyControl.xaml
    /// </summary>
    public partial class PropertyControl : UserControl
    {
        protected readonly PropertyInfo prop;
        protected readonly object source;

        public PropertyControl()
        {
            InitializeComponent();
        }

        public PropertyControl(object source, PropertyInfo prop)
        {
            InitializeComponent();
            this.source = source;
            this.prop = prop;
        }

        public string Label
        {
            get
            {
                return label.Text;
            }
            set
            {
                label.Text = value;
            }
        }

        public object Value { get; protected set; } = null;

        public void UpdateValue()
        {
            prop.SetValue(source, Value);
        }
    }
}
