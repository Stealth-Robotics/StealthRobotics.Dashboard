using StealthRobotics.Dashboard.API.Network;
using StealthRobotics.Dashboard.API.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        private void Tray_Expanded(object sender, EventArgs e)
        {
            TreeView netTree = tray.Content as TreeView;
            NetworkTree actualTree = NetworkUtil.GetTableOutline("SmartDashboard");
            netTree?.Items.Clear();
            netTree?.Items.Add(actualTree);
        }

        Point dragStartPoint;
        bool isStartingDrag = false;
        private void TreeView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            dragStartPoint = e.GetPosition(null);
            isStartingDrag = true;
        }

        private void TreeView_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);
            Vector motion = dragStartPoint - mousePos;

            if(e.LeftButton == MouseButtonState.Pressed && isStartingDrag &&
                (Math.Abs(motion.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(motion.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                TreeViewItem element = ((DependencyObject)e.OriginalSource).FindAncestor<TreeViewItem>();

                NetworkElement data = (NetworkElement)element.DataContext;

                DataObject dragInfo = new DataObject(NetworkDataFormats.NetworkElement, data);
                DragDrop.DoDragDrop(element, dragInfo, DragDropEffects.Copy | DragDropEffects.Move);
                tray.Show();
            }
        }

        private void TreeView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isStartingDrag = false;
        }

        private void DashboardRoot_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(NetworkDataFormats.NetworkElement)
                || e.Data.GetDataPresent(NetworkDataFormats.SourcedControl))
            {
                tray.Hide();
            }
        }

        private void DashboardRoot_Drop(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(NetworkDataFormats.SourcedControl))
            {

            }
            e.Handled = true;
        }

        private void DashboardRoot_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(NetworkDataFormats.SourcedControl))
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }
    }
}
