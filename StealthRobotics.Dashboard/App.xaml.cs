using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static System.Environment;

namespace StealthRobotics.Dashboard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //if testing, start the outlineviewer
            if (Debugger.IsAttached)
            {
                Process[] javas = Process.GetProcessesByName("java");
                bool isOutlineRunning = false;
                foreach (Process java in javas)
                {
                    isOutlineRunning = isOutlineRunning || java.MainWindowTitle == "OutlineViewer";
                }
                if (!isOutlineRunning)
                {
                    Process p = new Process();
                    p.StartInfo.FileName = "java";
                    string userprofile = Environment.GetFolderPath(SpecialFolder.UserProfile);
                    p.StartInfo.Arguments = $"-jar \"{userprofile}/wpilib/tools/OutlineViewer.jar\"";
                    p.Start();
                }
            }
        }
    }
}
