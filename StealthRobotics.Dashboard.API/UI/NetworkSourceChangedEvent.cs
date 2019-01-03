using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealthRobotics.Dashboard.API.UI
{
    public delegate void NetworkSourceChangedEventHandler(object sender, NetworkSourceChangedEventArgs e);

    public class NetworkSourceChangedEventArgs : EventArgs
    {
        public string NewSource { get; private set; }
        public string OldSource { get; private set; }

        public NetworkSourceChangedEventArgs(string oldSource, string newSource)
        {
            NewSource = newSource;
            OldSource = oldSource;
        }
    }
}
