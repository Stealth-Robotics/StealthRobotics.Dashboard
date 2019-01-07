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
            NetworkBinding.Initialize(0, Dispatcher, false);
            NetworkTables.NetworkTable.GetTable("").PutStringArray("CameraPublisher/Fake Camera 0/streams", new List<string>());
            System.Threading.Thread.Sleep(500);//give time to warm up so we can get data on the first go
            Button_Click(null, null);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            NetworkBinding.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            streams.Items.Clear();
            foreach(string cam in NetworkUtil.GetCameras())
            {
                streams.Items.Add(cam);
            }
        }

        private void Streams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            camera.StreamSource = NetworkUtil.GetCameraStreamURL(streams.SelectedItem as string);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (dashboardRoot.TileSizingMode == TileSizingMode.RowColumn)
                dashboardRoot.TileSizingMode = TileSizingMode.Uniform;
            else
                dashboardRoot.TileSizingMode = TileSizingMode.RowColumn;
        }
    }
}
