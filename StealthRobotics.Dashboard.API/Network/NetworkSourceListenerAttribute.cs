using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealthRobotics.Dashboard.API.Network
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class NetworkSourceListenerAttribute : Attribute
    {
        public Type[] SourceTypes { get; }

        public NetworkSourceListenerAttribute(params Type[] sourceTypes)
        {
            SourceTypes = sourceTypes;
        }
    }
}
