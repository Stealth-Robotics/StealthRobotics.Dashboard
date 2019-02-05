using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealthRobotics.Dashboard.Launcher.Parts
{
    [Export(typeof(IDashboardHandler))]
    [ExportMetadata("Label", "Java dashboard (test mode)")]
    public class JavaDashboard : IDashboardHandler
    {
        public void ClickAction()
        {
            ProgramStarter.Start("java -jar \"%userprofile%/wpilib/tools/SmartDashboard.jar\"");
        }
    }
}
