using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealthRobotics.Dashboard.Launcher.Parts
{
    [Export(typeof(IDashboardHandler))]
    [ExportMetadata("Label", "C# Dashboard")]
    public class CSharpDash : IDashboardHandler
    {
        public void ClickAction()
        {
            //install location will be local even after install
            ProgramStarter.Start("StealthRobotics.Dashboard.exe");
        }
    }
}
