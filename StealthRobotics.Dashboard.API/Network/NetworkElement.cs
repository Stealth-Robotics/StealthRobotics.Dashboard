using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealthRobotics.Dashboard.API
{
    /// <summary>
    /// Represents an entry in the network table without needing its value
    /// </summary>
    public class NetworkElement
    {
        /// <summary>
        /// The name of the entry
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// The type of the entry. Can be a primitive type or NetworkTable
        /// </summary>
        public readonly Type Type;
        internal NetworkElement(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public override string ToString()
        {
            return $"{Name} ({Type.Name})";
        }
    }
}
