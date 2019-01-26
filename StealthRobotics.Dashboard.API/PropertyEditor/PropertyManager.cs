using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StealthRobotics.Dashboard.API.PropertyEditor
{
    public static class PropertyManager
    {
        public static IEnumerable<PropertyInfo> GetEditorProperties(Type t)
        {
            return t.GetProperties().Where((x) =>
            {
                IEnumerable<Attribute> attrs = x.GetCustomAttributes<DialogPropertyAttribute>();
                return attrs.Count() > 0;
            });
        }
    }
}
