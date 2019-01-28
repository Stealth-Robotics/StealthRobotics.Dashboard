using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealthRobotics.Dashboard.IO
{
    public class LayoutSerializationBinder : DefaultSerializationBinder
    {
        public static LayoutSerializationBinder Instance = new LayoutSerializationBinder();

        public override Type BindToType(string assemblyName, string typeName)
        {
            //only allow serializing plugin types meaning that unloaded plugins just die
            IEnumerable<Type> controls = PluginLoader.GetControls();
            return controls.SingleOrDefault((t) =>
            {
                bool asmMatches = t.Assembly.FullName.Split(',')[0] == assemblyName;
                bool typeMatches = t.FullName == typeName;
                return asmMatches && typeMatches;
            });
        }
    }
}
