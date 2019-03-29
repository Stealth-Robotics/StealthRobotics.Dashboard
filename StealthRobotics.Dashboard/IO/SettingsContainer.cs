using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealthRobotics.Dashboard.IO
{
    public struct SettingsContainer
    {
        public int TeamNumber;
        public bool UsingDriverStation;

        public SettingsContainer(int teamNumber = 0, bool usingDriverStation = false)
        {
            TeamNumber = teamNumber;
            UsingDriverStation = usingDriverStation;
        }
    }
}
