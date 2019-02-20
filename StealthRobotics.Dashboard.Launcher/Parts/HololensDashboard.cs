using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
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
            //MessageBox.Show("New tech coming soon from Stealth Robotics.", "Oops! That's not ready!",
            //    MessageBoxButton.OK, MessageBoxImage.Information);
            //todo: check desktop, if not there, let the user know it needs to be.
            string dashboard = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\HoloDash.exe";
            if (File.Exists(dashboard))
            {
                ProgramStarter.Start($"\"{dashboard}\"");
            }
            else
            {
                MessageBox.Show("Couldn't find the dashboard. Make sure the dashboard is on the desktop and named HoloDash.exe.", 
                    "Oops! Couldn't find that.", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
