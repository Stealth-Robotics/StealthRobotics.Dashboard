using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StealthRobotics.Dashboard.API.PropertyEditor;
using StealthRobotics.Dashboard.API.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StealthRobotics.Dashboard.IO
{
    public class LayoutContractResolver : DefaultContractResolver
    {
        public static readonly LayoutContractResolver Instance = new LayoutContractResolver();

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IEnumerable<PropertyInfo> properties = PropertyManager.GetEditorProperties(type);
            List<JsonProperty> p = new List<JsonProperty>();
            foreach (PropertyInfo prop in properties)
            {
                p.Add(CreateProperty(prop, memberSerialization));
            }
            //if p is non-empty, also add the tilegrid row and column. be ready to change this if deserialize breaks
            if(p.Count > 0)
            {
                p.Add(new JsonProperty
                {
                    PropertyType = typeof(int),
                    PropertyName = "TileGrid.Row",
                    Readable = true,
                    Writable = true,
                    ValueProvider = new TileGridRowProvider()
                });
                p.Add(new JsonProperty
                {
                    PropertyType = typeof(int),
                    PropertyName = "TileGrid.Column",
                    Readable = true,
                    Writable = true,
                    ValueProvider = new TileGridColProvider()
                });
            }
            return p;
        }
    }
}
