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
        public int Count
        {
            get { return (int)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Count.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CountProperty =
            DependencyProperty.Register("Count", typeof(int), typeof(TestSourcesControl), new PropertyMetadata(0));

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

        public TestSourcesControl() : base()
        {
            StackPanel p = new StackPanel();
            Content = p;

            Button clicker = new Button();
            Binding countBind = new Binding("Count")
            {
                Source = this
            };
            BindingOperations.SetBinding(clicker, Button.ContentProperty, countBind);
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
