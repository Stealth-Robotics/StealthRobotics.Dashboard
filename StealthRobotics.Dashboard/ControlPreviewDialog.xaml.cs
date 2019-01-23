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
using System.Windows.Shapes;

namespace StealthRobotics.Dashboard
{
    /// <summary>
    /// Interaction logic for ControlPreviewWindow.xaml
    /// </summary>
    public partial class ControlPreviewDialog : Window
    {
        private Border selectedControl = null;

        private readonly SolidColorBrush blue = new SolidColorBrush(Colors.LightBlue);

        /// <summary>
        /// The finally selected type of control
        /// </summary>
        public Type SelectedType
        {
            get
            {
                StackPanel p = selectedControl?.Child as StackPanel;
                //the first child is the label, the second is a separator, and the third is the tilegrid
                TileGrid tileGrid = p?.Children.Cast<UIElement>().Last() as TileGrid;
                //the tilegrid's only child is the control!
                SourcedControl c = tileGrid?.Children.Cast<UIElement>().Last() as SourcedControl;
                //get the source control type info to pass to the dragdrop
                return c?.GetType();
            }
        }

        public ControlPreviewDialog()
        {
            InitializeComponent();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void AvailableControls_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Border b = ((DependencyObject)e.OriginalSource).FindAncestor<Border>();
            if (b == null) return;

            if (selectedControl != null)
                selectedControl.Background = null;
            b.Background = blue;
            selectedControl = b;
        }
    }
}
