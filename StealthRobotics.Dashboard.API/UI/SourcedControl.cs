using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using StealthRobotics.Dashboard.API.Network;
using StealthRobotics.Dashboard.API.PropertyEditor;

namespace StealthRobotics.Dashboard.API.UI
{
    public class SourcedControl : SnapControl
    {
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

            TextBlock label = new TextBlock();
            adorner.Children.Add(label);
            label.Margin = new Thickness(24, 2, 24, 2);
            label.SetBinding(TextBlock.TextProperty, this, "Label");
            label.SetBinding(TextBlock.HorizontalAlignmentProperty, this, "LabelHorizontalAlignment");
            label.SetBinding(TextBlock.VerticalAlignmentProperty, this, "LabelVerticalAlignment");
            label.SetBinding(TextBlock.VisibilityProperty, this, "LabelVisibility");

            AllowDrop = true;
            DragEnter += SourcedControl_DragEnter;
            Drop += SourcedControl_Drop;
        }

        private void SourcedControl_DragEnter(object sender, DragEventArgs e)
        {
            //assume we'll reject, otherwise we can fix it
            e.Effects = DragDropEffects.None;
            if (e.Data.GetDataPresent("NTSource"))
            {
                NetworkElement element = e.Data.GetData("NTSource") as NetworkElement;
                if(ValidateDropType(element))
                {
                    e.Effects = DragDropEffects.Copy;
                }
            }
        }

        private void SourcedControl_Drop(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent("NTSource"))
            {
                NetworkElement element = e.Data.GetData("NTSource") as NetworkElement;
                if (ValidateDropType(element))
                {
                    //source = full path
                    Source = element.FullPath.Replace("/SmartDashboard/", "");
                }
            }
        }

        private bool ValidateDropType(NetworkElement e)
        {
            if (e.Type == typeof(NetworkTree))
            {
                //deal with complex types - later
                throw new NotImplementedException();
            }
            else
            {
                //deal with primitives
                //get all the unique primitive types allowed
                IEnumerable<Type> allowedPrimitiveTypes = GetType()
                    .GetCustomAttributes(typeof(NetworkSourceListenerAttribute), true)
                    .Cast<NetworkSourceListenerAttribute>()
                    .SelectMany((listener) => listener.SourceTypes)
                    .Distinct();
                return allowedPrimitiveTypes.Contains(e.Type);
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            new PropertyDialog(this).ShowDialog();
        }
    }
}
