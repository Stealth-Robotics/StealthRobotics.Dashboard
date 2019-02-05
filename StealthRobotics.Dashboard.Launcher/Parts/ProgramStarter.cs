using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealthRobotics.Dashboard.Launcher.Parts
{
    public static class ProgramStarter
    {
        public static void Start(string commandLine)
        {
            string[] sections = commandLine.Split(new char[] { ' ' }, 2);
            Process p = new Process();
            p.StartInfo.FileName = sections[0];
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            //if there are arguments check them
            if (sections.Length > 1)
            {
                p.StartInfo.Arguments = Environment.ExpandEnvironmentVariables(sections[1]);
            }
            p.Start();
        }
    }
}
