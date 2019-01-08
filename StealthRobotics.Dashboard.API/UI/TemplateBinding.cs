using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace StealthRobotics.Dashboard.API.UI
{
    internal class TemplateBinding : Binding
    {
        public TemplateBinding(string path) : base(path)
        {
            RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent);
        }
    }
}
