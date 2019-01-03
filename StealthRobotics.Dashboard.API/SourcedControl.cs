﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace StealthRobotics.Dashboard.API
{
    public class SourcedControl : SnapControl
    {
        public event EventHandler SourceChanged;

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
            (d as SourcedControl)?.SourceChanged?.Invoke(d, new EventArgs());
        }

        public SourcedControl() : base()
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
        }

        private void Settings_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new PropertyDialog(this).ShowDialog();
        }
    }
}
