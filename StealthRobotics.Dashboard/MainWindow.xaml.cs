using Adorners;
using Microsoft.Win32;
using NetworkTables;
using StealthRobotics.Dashboard.API.Network;
using StealthRobotics.Dashboard.API.UI;
using StealthRobotics.Dashboard.Controls;
using StealthRobotics.Dashboard.IO;
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
        SettingsContainer settings;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //init network table and put test camera
            settings = SaveFileManager.LoadSettings();
            NetworkBinding.Initialize(settings.TeamNumber, Dispatcher, settings.UsingDriverStation);
            //NetworkTable.GetTable("").PutStringArray("CameraPublisher/Fake Camera 0/streams", new List<string>());
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            NetworkBinding.Shutdown();
            SaveFileManager.SaveSettings(settings);
        }

        private void RefreshSources()
        {
            if (NetworkBinding.IsRunning)
            {
                NetworkTree actualTree = NetworkUtil.GetTableOutline("SmartDashboard");
                netTree?.Items.Clear();
                netTree?.Items.Add(actualTree);
            }
        }

        private void PopulateControlPreviewWrapPanel(WrapPanel wp, IEnumerable<Type> controlTypes)
        {
            wp.Children.Clear();
            foreach (Type controlType in controlTypes)
            {
                //do some fancy arranging. what we need:
                //size-to-fit border (plus some extra space) with transparent background to register the hits.
                //label the extra space with the name of the control
                SourcedControl c = (SourcedControl)controlType.GetConstructor(Type.EmptyTypes).Invoke(null);
                c.IsEnabled = false;
                double expectedWidth = TileGrid.GetColumnSpan(c) * 50;
                double expectedHeight = TileGrid.GetRowSpan(c) * 50;
                TileGrid container = new TileGrid()
                {
                    ShowGridlines = false,
                    IsEditable = false,
                    TileSizingMode = TileSizingMode.Uniform,
                    Width = expectedWidth,
                    Height = expectedHeight,
                    Background = new SolidColorBrush(Colors.Transparent)
                };
                container.Children.Add(c);
                Border outline = new Border()
                {
                    Background = new SolidColorBrush(Colors.Transparent),
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(5)
                };
                StackPanel p = new StackPanel();
                outline.Child = p;
                TextBlock label = new TextBlock()
                {
                    Text = Util.ToTitleCase(controlType.Name),
                    Margin = new Thickness(2)
                };
                p.Children.Add(label);
                p.Children.Add(new Separator());
                p.Children.Add(container);
                wp.Children.Add(outline);
            }
        }

        private void RefreshControls()
        {
            IEnumerable<Type> controlTypes = PluginLoader.GetControls();
            PopulateControlPreviewWrapPanel(availableControls, controlTypes);
        }

        private void RefreshTrayDisplay()
        {
            if (sources.IsSelected)
            {
                RefreshSources();
            }
            else if(controls.IsSelected)
            {
                RefreshControls();
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshTrayDisplay();
        }

        private void Tray_Expanded(object sender, EventArgs e)
        {
            RefreshTrayDisplay();
        }

        Point treeDragStartPoint;
        private void TreeView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            treeDragStartPoint = e.GetPosition(null);
        }

        private void TreeView_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);
            Vector motion = treeDragStartPoint - mousePos;

            if(e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(motion.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(motion.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                TreeViewItem element = ((DependencyObject)e.OriginalSource).FindAncestor<TreeViewItem>();
                if (element == null) return;

                NetworkElement data = (NetworkElement)element.DataContext;

                DataObject dragInfo = new DataObject(NetworkDataFormats.NetworkElement, data);
                DragDrop.DoDragDrop(element, dragInfo, DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link);
                tray.Show();
            }
        }

        Point controlDragStartPoint;
        private void AvailableControls_MouseDown(object sender, MouseButtonEventArgs e)
        {
            controlDragStartPoint = e.GetPosition(null);
        }

        private void AvailableControls_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);
            Vector motion = controlDragStartPoint - mousePos;

            if(e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(motion.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(motion.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                Border source = ((DependencyObject)e.OriginalSource).FindAncestor<Border>();
                if (source == null) return;

                StackPanel p = source.Child as StackPanel;
                //the first child is the label, the second is a separator, and the third is the tilegrid
                TileGrid tileGrid = p.Children.Cast<UIElement>().Last() as TileGrid;
                //the tilegrid's only child is the control!
                SourcedControl c = tileGrid.Children.Cast<UIElement>().Last() as SourcedControl;
                //get the source control type info to pass to the dragdrop
                Type controlType = c.GetType();
                DataObject dragInfo = new DataObject(NetworkDataFormats.SourcedControl, controlType);
                DragDrop.DoDragDrop(source, dragInfo, DragDropEffects.Copy | DragDropEffects.Move);
                tray.Show();
            }
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
            if(e.KeyStates.HasFlag(DragDropKeyStates.ControlKey) &&
                e.Data.GetDataPresent(NetworkDataFormats.NetworkElement))
            {
                //offer to create the bound control here
                NetworkElement dataSource = (NetworkElement)e.Data.GetData(NetworkDataFormats.NetworkElement);
                //all available types
                IEnumerable<Type> controlTypes = PluginLoader.GetControls();
                IEnumerable<Type> allowedControls;
                //check whether this is a primitive or complex data and filter the controls appropriately
                if(dataSource.Type == typeof(NetworkTree))
                {
                    //get the expected type of the element
                    NetworkTable table = NetworkTable.GetTable(dataSource.FullPath);
                    string dataType = table.GetString("type", null);
                    allowedControls = controlTypes
                        .Where((t) => SourcedControl.GetAllowedComplexTypes(t).Contains(dataType));
                }
                else
                {
                    allowedControls = controlTypes
                        .Where((t) => SourcedControl.GetAllowedPrimitiveTypes(t).Contains(dataSource.Type));
                }
                if (allowedControls.Count() != 0)
                {
                    ControlPreviewDialog previewDialog = new ControlPreviewDialog();
                    PopulateControlPreviewWrapPanel(previewDialog.availableControls, allowedControls);
                    if (previewDialog.ShowDialog() == true)
                    {
                        Type controlType = previewDialog.SelectedType;
                        if (controlType != null)
                        {
                            SourcedControl c = (SourcedControl)controlType.GetConstructor(Type.EmptyTypes).Invoke(null);
                            PlaceAtDropPoint(c, e);
                            dashboardRoot.Children.Add(c);
                            c.Source = dataSource.FullPath.Replace("/SmartDashboard/", "");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("There are no controls that can display this entry!", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else if(e.Data.GetDataPresent(NetworkDataFormats.SourcedControl))
            {
                //create the control here
                Type controlType = (Type)e.Data.GetData(NetworkDataFormats.SourcedControl);
                SourcedControl c = (SourcedControl)controlType.GetConstructor(Type.EmptyTypes).Invoke(null);
                PlaceAtDropPoint(c, e);
                dashboardRoot.Children.Add(c);
            }
            e.Handled = true;
        }

        private void DashboardRoot_DragOver(object sender, DragEventArgs e)
        {
            if((e.KeyStates.HasFlag(DragDropKeyStates.ControlKey) &&
                e.Data.GetDataPresent(NetworkDataFormats.NetworkElement)) ||
                e.Data.GetDataPresent(NetworkDataFormats.SourcedControl))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void PlaceAtDropPoint(SourcedControl c, DragEventArgs e)
        {
            Point dropPoint = e.GetPosition(dashboardRoot);
            //centering. goal: if there span is even, we want to center on nearest line,
            //if span is odd, center the nearest full tile
            //for even, rounding works because the division causes the control to tend towards the left
            int colSpan = TileGrid.GetColumnSpan(c);
            int rowSpan = TileGrid.GetRowSpan(c);
            double preciseCol = dropPoint.X / dashboardRoot.GetColumnWidth();
            double preciseRow = dropPoint.Y / dashboardRoot.GetRowHeight();
            int col = colSpan % 2 == 0 ? (int)Math.Round(preciseCol) : (int)Math.Floor(preciseCol);
            int row = rowSpan % 2 == 0 ? (int)Math.Round(preciseRow) : (int)Math.Floor(preciseRow);
            int colOffset = colSpan / 2;
            int rowOffset = rowSpan / 2;
            col = Math.Max(col - colOffset, 0);
            row = Math.Max(row - rowOffset, 0);
            TileGrid.SetColumn(c, col);
            TileGrid.SetRow(c, row);
        }

        private void MenuItem_Checked(object sender, RoutedEventArgs e)
        {
            tray.Hide();
            tray.HideAdorner();
            //binding being weird
            contextComp.IsChecked = true;
            menuComp.IsChecked = true;
            dashboardRoot.IsEditable = false;
        }

        private void MenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            tray.ShowAdorner();
            //binding being weird
            contextComp.IsChecked = false;
            menuComp.IsChecked = false;
            dashboardRoot.IsEditable = true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog()
            {
                Filter = "Dashboard layouts|*.dbl"
            };
            if (dlg.ShowDialog() == true)
            {
                SaveFileManager.SaveLayout(dlg.FileName, dashboardRoot.Children);
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                Filter = "Dashboard layouts|*.dbl"
            };
            if (dlg.ShowDialog() == true)
            {
                SaveFileManager.LoadLayout(dlg.FileName, dashboardRoot.Children);
            }
        }

        private void Plugins_Click(object sender, RoutedEventArgs e)
        {
            //make sure plugins are unloaded from the preview window in case needed
            //hide to cover it up
            tray.Hide();
            availableControls.Children.Clear();
            new PluginManagerDialog().ShowDialog();
        }

        private void TeamConfig_Click(object sender, RoutedEventArgs e)
        {
            TeamSettingsDialog teamSettings = new TeamSettingsDialog(settings);
            if (teamSettings.ShowDialog() == true)
            {
                settings = teamSettings.GetSettings();
                NetworkBinding.Refresh(settings.TeamNumber, settings.UsingDriverStation);
            }
        }
    }
}