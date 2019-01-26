using Adorners;
using StealthRobotics.Dashboard.API.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace StealthRobotics.Dashboard.Controls
{
    public class LeftSideTray : AdornedControl
    {
        //resources
        private readonly SolidColorBrush lightGray = new SolidColorBrush(Colors.LightGray);
        private readonly SolidColorBrush lightBlue = new SolidColorBrush(Colors.LightBlue);
        private readonly SolidColorBrush darkGray = new SolidColorBrush(Colors.DarkGray);
        private readonly SolidColorBrush stealthGray = new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80));

        //content
        private Button adorner;

        //other stuff
        private const double maxSlideInTime = 0.25;
        private const double maxSlideOutTime = 0.25;
        private ThicknessAnimation slideOut = null;
        private Thickness defaultMargin;

        public event EventHandler Expanded;
        public event EventHandler Hidden;

        public LeftSideTray()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            //build content
            InitializeContent();

            //finalize this setup
            AdornerContent = adorner;
            IsAdornerVisible = true;
            HorizontalAdornerPlacement = AdornerPlacement.Outside;
            Loaded += AdornedControl_Loaded;
        }

        private void InitializeContent()
        {
            adorner = new Button()
            {
                Width = 20,
                Height = 50,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            adorner.Click += Adorner_Click;
            Style buttonStyling = new Style(typeof(Button));
            buttonStyling.Setters.Add(new Setter(Button.BackgroundProperty, lightGray));
            Trigger hoverTrigger = new Trigger() { Property = Button.IsMouseOverProperty, Value = true };
            hoverTrigger.Setters.Add(new Setter(Button.BackgroundProperty, lightBlue));
            buttonStyling.Triggers.Add(hoverTrigger);
            Trigger clickTrigger = new Trigger() { Property = Button.IsPressedProperty, Value = true };
            clickTrigger.Setters.Add(new Setter(Button.BackgroundProperty, darkGray));
            buttonStyling.Triggers.Add(clickTrigger);
            adorner.Style = buttonStyling;
            Path buttonContent = new Path() { StrokeThickness = 2, Stroke = stealthGray };
            buttonContent.Data = Geometry.Parse("M 4,17.5 v 15 m 5,0 v -15 m 5,0 v 15");
            adorner.Content = buttonContent;
            ControlTemplate buttonTheme = (ControlTemplate)XamlReader.Parse(
                @"<ControlTemplate TargetType=""Button"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                    <Border CornerRadius=""0 5 5 0"" Background=""{TemplateBinding Background}"">
                        <ContentPresenter/>
                    </Border>
                </ControlTemplate>");
            adorner.Template = buttonTheme;

            ControlTemplate contentTheme = (ControlTemplate)XamlReader.Parse(
                @"<ControlTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                                   xmlns:a=""clr-namespace:Adorners;assembly=AdornedControl""
                                   xmlns:local=""clr-namespace:StealthRobotics.Dashboard.Controls;assembly=StealthRobotics.Dashboard""
                                   TargetType=""local:LeftSideTray"">
                    <Border Background=""White"" BorderBrush=""Gray"" BorderThickness=""1"">
                        <ContentPresenter/>
                    </Border>
                </ControlTemplate>");
            Template = contentTheme;
        }
        
        private void Adorner_Click(object sender, RoutedEventArgs e)
        {
            if (slideOut == null)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        public void Show()
        {
            //slide out
            double slideTime = maxSlideOutTime * GetSlidePercent();
            slideOut = new ThicknessAnimation(new Thickness(0), new Duration(TimeSpan.FromSeconds(slideTime)));
            BeginAnimation(MarginProperty, slideOut);
            Expanded?.Invoke(this, new EventArgs());
        }

        public void Hide()
        {
            //cancel the slide out
            slideOut = null;
            //slide out
            double slideTime = maxSlideInTime * (1 - GetSlidePercent());
            ThicknessAnimation slideIn = new ThicknessAnimation(defaultMargin, new Duration(TimeSpan.FromSeconds(slideTime)));
            BeginAnimation(MarginProperty, slideIn);
            Hidden?.Invoke(this, new EventArgs());
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
