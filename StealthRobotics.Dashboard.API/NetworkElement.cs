using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealthRobotics.Dashboard.API
{
    public class NetworkElement
    {
        public readonly string Name;
        public readonly Type Type;
        protected NetworkElement(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }
}
