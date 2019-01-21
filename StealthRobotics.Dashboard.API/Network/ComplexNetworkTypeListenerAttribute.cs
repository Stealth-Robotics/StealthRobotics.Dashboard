using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealthRobotics.Dashboard.API.Network
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ComplexNetworkListenerAttribute : Attribute
    {
        public string[] TableTypes { get; }

        public ComplexNetworkListenerAttribute(params string[] tableTypes)
        {
            TableTypes = tableTypes;
        }
    }
}
