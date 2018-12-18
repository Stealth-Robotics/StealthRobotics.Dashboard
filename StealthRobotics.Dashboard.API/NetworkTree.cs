using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkTables;

namespace StealthRobotics.Dashboard.API
{
    public class NetworkTree
    {
        private class Node
        {
            public readonly string Name;
            public readonly Type Type;
            public List<Node> Children;
            public Node(string name, Type type)
            {
                Name = name;
                Type = type;
                Children = new List<Node>();
            }
        }

        //be mindful of this, access good, don't expose node if possible. However, we do need to expose all data from node
        //possible restructure?
        private readonly Node root;

        internal NetworkTree(string root)
        {
            this.root = new Node(root, typeof(NetworkTable));
            ConstructChildren(this.root);
        }

        private void ConstructChildren(Node root)
        {

        }
    }
}
