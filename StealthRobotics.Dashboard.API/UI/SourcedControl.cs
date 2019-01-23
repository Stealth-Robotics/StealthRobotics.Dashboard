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
using System.Windows.Shapes;
using NetworkTables;
using StealthRobotics.Dashboard.API.Network;
using StealthRobotics.Dashboard.API.PropertyEditor;

namespace StealthRobotics.Dashboard.API.UI
{
    public class SourcedControl : SnapControl
    {
        /// <summary>
        /// Gets all the primitive data types that a given SourcedControl type can handle
        /// </summary>
        /// <param name="t">The SourcedControl type to check</param>
        /// <remarks>Throws an InvalidOperationException if t is not a descendant of SourcedControl</remarks>
        public static IEnumerable<Type> GetAllowedPrimitiveTypes(Type t)
        {
            if (!typeof(SourcedControl).IsAssignableFrom(t))
                throw new InvalidOperationException("Can't define allowed types on non-SourceControl objects.");
            IEnumerable<Type> allowedPrimitiveTypes = t
                .GetCustomAttributes(typeof(NetworkSourceListenerAttribute), true)
                .Cast<NetworkSourceListenerAttribute>()
                .SelectMany((listener) => listener.SourceTypes)
                .Distinct();
            return allowedPrimitiveTypes;
        }

        /// <summary>
        /// Gets all the complex data types that a given SourcedControl type can handle
        /// </summary>
        /// <param name="t">The SourcedControl type to check</param>
        /// <remarks>Throws an InvalidOperationException if t is not a descendant of SourcedControl</remarks>
        public static IEnumerable<string> GetAllowedComplexTypes(Type t)
        {
            if (!typeof(SourcedControl).IsAssignableFrom(t))
                throw new InvalidOperationException("Can't define allowed types on non-SourceControl objects.");
            IEnumerable<string> allowedTypes = t
                .GetCustomAttributes(typeof(ComplexNetworkListenerAttribute), true)
                .Cast<ComplexNetworkListenerAttribute>()
                .SelectMany((listener) => listener.TableTypes)
                .Distinct();
            return allowedTypes;
        }

        public event NetworkSourceChangedEventHandler SourceChanged;

        [DialogProperty]
        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(SourcedControl), new FrameworkPropertyMetadata("", OnSourceChanged));

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SourcedControl).SourceChanged?.Invoke(d, new NetworkSourceChangedEventArgs((string)e.OldValue, (string)e.NewValue));
        }

        [DialogProperty]
        public Visibility LabelVisibility
        {
            get { return (Visibility)GetValue(LabelVisibilityProperty); }
            set { SetValue(LabelVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelVisibilityProperty =
            DependencyProperty.Register("LabelVisibility", typeof(Visibility), typeof(SourcedControl), new PropertyMetadata(Visibility.Hidden));

        [DialogProperty]
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(SourcedControl), new PropertyMetadata(""));

        [DialogProperty]
        public HorizontalAlignment LabelHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(LabelHorizontalAlignmentProperty); }
            set { SetValue(LabelHorizontalAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelHorizontalAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelHorizontalAlignmentProperty =
            DependencyProperty.Register("LabelHorizontalAlignment", typeof(HorizontalAlignment),
                typeof(SourcedControl), new PropertyMetadata(HorizontalAlignment.Left));

        [DialogProperty]
        public VerticalAlignment LabelVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(LabelVerticalAlignmentProperty); }
            set { SetValue(LabelVerticalAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelVerticalAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelVerticalAlignmentProperty =
            DependencyProperty.Register("LabelVerticalAlignment", typeof(VerticalAlignment),
                typeof(SourcedControl), new PropertyMetadata(VerticalAlignment.Top));
        
        public SourcedControl()
        {
            Button settings = new Button();
            settings.Click += Settings_Click;
            BitmapImage gear = new BitmapImage();
            gear.BeginInit();
            gear.UriSource = new Uri(@"pack://application:,,,/StealthRobotics.Dashboard.API;component/assets/gear.png");
            gear.EndInit();
            Image i = new Image
            {
                Source = gear
            };
            settings.Content = i;
            PopupButtons.Add(settings);

            Button close = new Button();
            close.Click += Close_Click;
            Path x = new Path()
            {
                //same color as the gear
                Stroke = new SolidColorBrush(Color.FromRgb(51, 51, 51)),
                Data = Geometry.Parse("M 3 3 L 13 13 M 3 13 L 13 3"),
                StrokeThickness = 3,
                Width = 16,
                Height = 16
            };
            close.Content = x;
            PopupButtons.Add(close);

            TextBlock label = new TextBlock();
            adorner.Children.Add(label);
            label.Margin = new Thickness(24, 2, 24, 2);
            label.SetBinding(TextBlock.TextProperty, this, "Label");
            label.SetBinding(TextBlock.HorizontalAlignmentProperty, this, "LabelHorizontalAlignment");
            label.SetBinding(TextBlock.VerticalAlignmentProperty, this, "LabelVerticalAlignment");
            label.SetBinding(TextBlock.VisibilityProperty, this, "LabelVisibility");

            AllowDrop = true;
            DragOver += SourcedControl_DragOver;
            Drop += SourcedControl_Drop;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (LogicalTreeHelper.GetParent(this) is Panel parent)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to remove this control?",
                    "Confirm deletion", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    //this should unbind the network table
                    Source = null;
                    parent.Children.Remove(this);
                }
            }
        }

        private void SourcedControl_DragOver(object sender, DragEventArgs e)
        {
            //assume we'll reject, otherwise we can fix it
            if (e.Data.GetDataPresent(NetworkDataFormats.NetworkElement))
            {
                NetworkElement element = e.Data.GetData(NetworkDataFormats.NetworkElement) as NetworkElement;
                if (ValidateDropType(element))
                {
                    e.Effects = DragDropEffects.Link;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
            }
            e.Handled = true;
        }

        private void SourcedControl_Drop(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(NetworkDataFormats.NetworkElement))
            {
                NetworkElement element = e.Data.GetData(NetworkDataFormats.NetworkElement) as NetworkElement;
                if (ValidateDropType(element))
                {
                    //source = full path
                    Source = element.FullPath.Replace("/SmartDashboard/", "");
                    //only handle if successful drop
                    e.Handled = true;
                }
            }
        }

        private bool ValidateDropType(NetworkElement e)
        {
            if (e.Type == typeof(NetworkTree))
            {
                //deal with complex types
                NetworkTable table = NetworkTable.GetTable(e.FullPath);
                string dataType = table.GetString("type", null);
                return GetAllowedComplexTypes(GetType()).Contains(dataType);
            }
            else
            {
                //deal with primitives
                //get all the unique primitive types allowed
                return GetAllowedPrimitiveTypes(GetType()).Contains(e.Type);
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            new PropertyDialog(this).ShowDialog();
        }
    }
}
