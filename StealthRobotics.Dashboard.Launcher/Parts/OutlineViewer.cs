﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealthRobotics.Dashboard.Launcher.Parts
{
    [Export(typeof(IDashboardHandler))]
    [ExportMetadata("Label", "Outline viewer")]
    public class OutlineViewer : IDashboardHandler
    {
        public void ClickAction()
        {
            ProgramStarter.Start("java -jar \"%userprofile%/wpilib/tools/OutlineViewer.jar\"");
        }
    }
}
