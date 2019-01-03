using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealthRobotics.Dashboard.API.PropertyEditor
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class DialogPropertyAttribute : Attribute
    {
        public DialogPropertyAttribute() { }
    }
}
