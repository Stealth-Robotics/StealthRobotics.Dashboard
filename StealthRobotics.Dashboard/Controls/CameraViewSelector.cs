using StealthRobotics.Dashboard.API.Network;
using StealthRobotics.Dashboard.API.PropertyEditor;
using StealthRobotics.Dashboard.API.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StealthRobotics.Dashboard.Controls
{
    public class CameraViewSelector : SourcedControl
    {
        //we don't actually want a source in the editor, so sneakily hide it
        private new string Source { get; set; }

        [DialogProperty]
        public int DisplayPanelWidth
        {
            get { return (int)GetValue(DisplayPanelWidthProperty); }
            set { SetValue(DisplayPanelWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PaneWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayPanelWidthProperty =
            DependencyProperty.Register("DisplayPanelWidth", typeof(int), typeof(CameraViewSelector), new PropertyMetadata(7));

        [DialogProperty]
        public int DisplayPanelHeight
        {
            get { return (int)GetValue(DisplayPanelHeightProperty); }
            set { SetValue(DisplayPanelHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PaneHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayPanelHeightProperty =
            DependencyProperty.Register("DisplayPanelHeight", typeof(int), typeof(CameraViewSelector), new PropertyMetadata(5, OnPanelHeightChanged));

        private static void OnPanelHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CameraViewSelector).BuildRows();
        }

        Grid layout;
        CameraStream stream;
        ComboBox streams;

        public CameraViewSelector()
        {
            layout = new Grid();
            BuildRows();
            Content = layout;

            stream = new CameraStream() { Stretch = Stretch.Uniform };
            layout.Children.Add(stream);

            StackPanel chooserArea = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            streams = new ComboBox()
            {
                MinWidth = 150,
                Margin = new Thickness(0, 10, 2.5, 10)
            };
            streams.SelectionChanged += Streams_SelectionChanged;
            chooserArea.Children.Add(streams);
            Button refresh = new Button()
            {
                Margin = new Thickness(2.5, 10, 0, 10),
                Padding = new Thickness(5),
                Content = "Refresh"
            };
            refresh.Click += Refresh_Click;
            chooserArea.Children.Add(refresh);
            Grid.SetRow(chooserArea, 1);
            layout.Children.Add(chooserArea);

            AddOneConverter extraRows = new AddOneConverter();
            Binding rowBinding = new Binding("DisplayPanelHeight")
            {
                Source = this,
                Converter = extraRows
            };
            BindingOperations.SetBinding(this, TileGrid.RowSpanProperty, rowBinding);

            this.SetBinding(TileGrid.ColumnSpanProperty, this, "DisplayPanelWidth");
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            streams.Items.Clear();
            foreach (string cam in NetworkUtil.GetCameras())
            {
                streams.Items.Add(cam);
            }
        }

        private void Streams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            stream.StreamSource = NetworkUtil.GetCameraStreamURL(streams.SelectedItem as string);
        }

        private void BuildRows()
        {
            RowDefinition camRow = new RowDefinition() { Height = new GridLength(DisplayPanelHeight, GridUnitType.Star) };
            RowDefinition chooseRow = new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) };
            layout.RowDefinitions.Clear();
            layout.RowDefinitions.Add(camRow);
            layout.RowDefinitions.Add(chooseRow);
        }
    }
}
