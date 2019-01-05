using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using StealthRobotics.Dashboard.API.PropertyEditor;

namespace StealthRobotics.Dashboard.API.UI
{
    public class TestSourcesControl : SourcedControl
    {
        [DialogProperty]
        public uint Count
        {
            get { return (uint)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Count.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CountProperty =
            DependencyProperty.Register("Count", typeof(uint), typeof(TestSourcesControl), new PropertyMetadata((uint)0));

        [DialogProperty]
        public double Slidey
        {
            get { return (double)GetValue(SlideyProperty); }
            set { SetValue(SlideyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Slidey.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SlideyProperty =
            DependencyProperty.Register("Slidey", typeof(double), typeof(TestSourcesControl), new PropertyMetadata(0.0));

        public enum Cards
        {
            AceOfDiamonds, JackOfDiamonds, QueenOfDiamonds, KingOfDiamonds,
            AceOfSpades, JackOfSpades, QueenOfSpades, KingOfSpades,
            AceOfClubs, JackOfClubs, QueenOfClubs, KingOfClubs,
            AceOfHearts, JackOfHearts, QueenOfHearts, KingOfHearts
        }

        [DialogProperty]
        public Cards Card
        {
            get { return (Cards)GetValue(CardProperty); }
            set { SetValue(CardProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Suit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardProperty =
            DependencyProperty.Register("Card", typeof(Cards), typeof(TestSourcesControl), new PropertyMetadata(Cards.AceOfSpades));

        [DialogProperty]
        public bool EnableButton
        {
            get { return (bool)GetValue(EnableChildrenProperty); }
            set { SetValue(EnableChildrenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableChildren.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableChildrenProperty =
            DependencyProperty.Register("EnableChildren", typeof(bool), typeof(TestSourcesControl), new PropertyMetadata(true));

        public TestSourcesControl() : base()
        {
            StackPanel p = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center
            };
            Content = p;

            Button clicker = new Button();
            Binding countBind = new Binding("Count") { Source = this };
            BindingOperations.SetBinding(clicker, Button.ContentProperty, countBind);
            Binding enableBinding = new Binding("EnableChildren") { Source = this };
            BindingOperations.SetBinding(clicker, Button.IsEnabledProperty, enableBinding);
            clicker.Click += Clicker_Click;

            Slider slider = new Slider
            {
                Minimum = -10,
                Maximum = 10
            };
            Binding slideBind = new Binding("Slidey")
            {
                Source = this
            };
            BindingOperations.SetBinding(slider, Slider.ValueProperty, slideBind);

            p.Children.Add(clicker);
            p.Children.Add(slider);
        }

        private void Clicker_Click(object sender, RoutedEventArgs e)
        {
            Count++;
        }
    }
}
