using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealthRobotics.Dashboard.API.Network
{
    /// <summary>
    /// Represents an entry in the network table without needing its value
    /// </summary>
    public class NetworkElement
    {
        /// <summary>
        /// The name of the entry
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// The full path to this item in the network table
        /// </summary>
        public string FullPath { get; private set; }
        /// <summary>
        /// The type of the entry. Can be a primitive type or NetworkTable
        /// </summary>
        public readonly Type Type;
        internal NetworkElement(string name, Type type, NetworkTree parent = null)
        {
            Name = name;
            Type = type;
            if (parent != null)
                FullPath = parent.FullPath + "/" + name;
            else
                FullPath = name;
        }

        public override string ToString()
        {
            return $"{Name} ({Type.Name})";
        }
    }
}
