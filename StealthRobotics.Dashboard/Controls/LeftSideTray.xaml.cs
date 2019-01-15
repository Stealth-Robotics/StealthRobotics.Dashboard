using Adorners;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StealthRobotics.Dashboard.API.UI;

namespace StealthRobotics.Dashboard.Controls
{
    /// <summary>
    /// Interaction logic for LeftSideTray.xaml
    /// </summary>
    public partial class LeftSideTray : AdornedControl
    {
        private const double maxSlideInTime = 0.25;
        private const double maxSlideOutTime = 0.25;
        private ThicknessAnimation slideOut = null;
        private Thickness defaultMargin;

        public LeftSideTray()
        {
            InitializeComponent();
        }

        private void Adorner_Click(object sender, RoutedEventArgs e)
        {
            this.BringToFront();
            if(slideOut == null)
            {
                //slide out
                double slideTime = maxSlideOutTime * GetSlidePercent();
                slideOut = new ThicknessAnimation(new Thickness(0), new Duration(TimeSpan.FromSeconds(slideTime)));
                BeginAnimation(MarginProperty, slideOut);
            }
            else
            {
                //cancel the slide out
                slideOut = null;
                //slide out
                double slideTime = maxSlideInTime * (1 - GetSlidePercent());
                ThicknessAnimation slideIn = new ThicknessAnimation(defaultMargin, new Duration(TimeSpan.FromSeconds(slideTime)));
                BeginAnimation(MarginProperty, slideIn);
            }
        }

        private double GetSlidePercent()
        {
            double widthDivisor = ActualWidth > 0 ? ActualWidth : 1;
            return -Margin.Left / widthDivisor;
        }

        private void AdornedControl_Loaded(object sender, RoutedEventArgs e)
        {
            Margin = new Thickness(-ActualWidth, 0, 0, 0);
            defaultMargin = Margin;
        }
    }
}
