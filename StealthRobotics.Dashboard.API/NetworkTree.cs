using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkTables;

namespace StealthRobotics.Dashboard.API
{
    public class NetworkTree : NetworkElement
    {
        private List<NetworkElement> children;
        public IList<NetworkElement> Children
        {
            get
            {
                return children.AsReadOnly();
            }
        }
        internal NetworkTree(string root) : base(root, typeof(NetworkTable))
        {
            ConstructChildren(root);
        }

        private void ConstructChildren(string root)
        {
            children = new List<NetworkElement>();
            //todo: fill children
        }
    }
}
