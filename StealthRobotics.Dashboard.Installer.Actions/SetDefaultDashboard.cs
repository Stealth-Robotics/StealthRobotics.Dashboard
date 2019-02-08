using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace StealthRobotics.Dashboard.Installer.Actions
{
    [RunInstaller(true)]
    public partial class SetDefaultDashboard : System.Configuration.Install.Installer
    {
        public SetDefaultDashboard()
        {
            InitializeComponent();
        }

        string GetOldAndReplaceCommandLine(string newCmd)
        {
            const string dbConfig = @"C:\Users\Public\Documents\FRC\FRC DS Data Storage.ini";
            const string commandLineRegex = "DashboardCmdLine = (.*)\r?\n?";
            string oldFileContent = null;
            string oldCommandLine = null;
            //get the existing operation in case we need to rollback
            using (FileStream config = File.OpenRead(dbConfig))
            {
                using (StreamReader read = new StreamReader(config))
                {
                    oldFileContent = read.ReadToEnd();
                    oldCommandLine = Regex.Match(oldFileContent, commandLineRegex).Groups[1].Value;
                }
            }
            //for debugging, rename the file
            File.Delete(dbConfig + ".old");
            File.Move(dbConfig, dbConfig + ".old");
            //replace and write back
            string newFileContent = Regex.Replace(oldFileContent, commandLineRegex, "DashboardCmdLine = " + newCmd + "\r\n");
            using (FileStream config = File.Create(dbConfig))
            {
                using (StreamWriter write = new StreamWriter(config))
                {
                    write.Write(newFileContent);
                }
            }
            return oldCommandLine;
        }

        [SecurityPermission(SecurityAction.Demand)]
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            //compute the new location
            string installPath = Path.GetDirectoryName(Context.Parameters["assemblypath"]);
            string launcherPath = Path.Combine(installPath, "StealthRobotics.Dashboard.Launcher.exe");
            string newCommandLine = "\"\"" + launcherPath + "\"\"";
            stateSaver["newCommandLine"] = newCommandLine;
            //replace the location and save the old one for rollback
            stateSaver["oldCommandLine"] = GetOldAndReplaceCommandLine(newCommandLine);
        }

        [SecurityPermission(SecurityAction.Demand)]
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
        }

        [SecurityPermission(SecurityAction.Demand)]
        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
            //something has gone wrong, put back the original
            string oldCommandLine = (string)savedState["oldCommandLine"];
            GetOldAndReplaceCommandLine(oldCommandLine);
        }

        [SecurityPermission(SecurityAction.Demand)]
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
            //our dashboard is going away, put back the original
            string oldCommandLine = (string)savedState["oldCommandLine"];
            GetOldAndReplaceCommandLine(oldCommandLine);
        }
    }
}
