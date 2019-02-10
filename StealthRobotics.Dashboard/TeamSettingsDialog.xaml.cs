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
    /// Interaction logic for TeamSettingsDialog.xaml
    /// </summary>
    public partial class TeamSettingsDialog : Window
    {
        public int Team { get; private set; }

        public bool UseDriverStation { get; private set; }

        public TeamSettingsDialog()
        {
            InitializeComponent();
        }

        public TeamSettingsDialog(int team, bool driverStation)
        {
            InitializeComponent();
            teamNum.Value = team;
            useDS.IsChecked = driverStation;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void UseDS_Toggled(object sender, RoutedEventArgs e)
        {
            UseDriverStation = useDS.IsChecked ?? false;
        }

        private void TeamNum_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Team = teamNum.Value ?? 0;
        }
    }
}
