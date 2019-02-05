using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StealthRobotics.Dashboard.Launcher.Parts
{
    [Export(typeof(IDashboardHandler))]
    [ExportMetadata("Label", "Hololens Dashboard")]
    public class HololensDashboard : IDashboardHandler
    {
        public void ClickAction()
        {
            MessageBox.Show("New tech coming soon from Stealth Robotics.", "Oops! That's not ready!",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
