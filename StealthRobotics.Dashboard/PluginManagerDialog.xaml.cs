using Microsoft.Win32;
using StealthRobotics.Dashboard.IO;
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
    /// Interaction logic for PluginManagerDialog.xaml
    /// </summary>
    public partial class PluginManagerDialog : Window
    {
        public PluginManagerDialog()
        {
            InitializeComponent();
            ListItems();
        }

        private void ListItems()
        {
            PluginList.ItemsSource = PluginLoader.GetLoadedPlugins();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                Filter = "Plugin files|*.dll"
            };
            if(dlg.ShowDialog() == true)
            {
                PluginLoader.LoadPlugin(dlg.FileName);
                ListItems();
            }
        }

        private void Unload_Click(object sender, RoutedEventArgs e)
        {
            if(PluginList.SelectedItem is string plugin)
            {
                PluginLoader.UnloadPlugin(plugin);
                ListItems();
            }
        }
    }
}
