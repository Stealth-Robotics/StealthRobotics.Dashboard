using System;
using System.Collections.Generic;
using System.Linq;
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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace StealthRobotics.Dashboard.Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [ImportMany]
        IEnumerable<Lazy<IDashboardHandler, IDashboardHandlerMetadata>> dashboards;

        AssemblyCatalog _catalog;
        CompositionContainer _container;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //build a set of dashboard parts
            _catalog = new AssemblyCatalog(typeof(MainWindow).Assembly);
            _container = new CompositionContainer(_catalog);
            _container.ComposeParts(this);
            //take each part and put it in the window
            //todo: we may care about a specific order, figure out how to fix that
            foreach(var dashboard in dashboards)
            {
                Button b = new Button()
                {
                    Content = dashboard.Metadata.Label
                };
                b.Click += (obj, args) => dashboard.Value.ClickAction();
                LayoutRoot.Children.Add(b);
            }
            WindowState = WindowState.Normal;
            Activate();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //clean up
            _container.Dispose();
            _catalog.Dispose();
        }
    }
}
