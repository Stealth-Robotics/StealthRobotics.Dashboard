using StealthRobotics.Dashboard.API.Network;
using StealthRobotics.Dashboard.API.UI;
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

namespace StealthRobotics.Dashboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //init network table and put test camera
            NetworkBinding.Initialize(0, Dispatcher, false);
            NetworkTables.NetworkTable.GetTable("").PutStringArray("CameraPublisher/Fake Camera 0/streams", new List<string>());
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            NetworkBinding.Shutdown();
        }
    }
}
