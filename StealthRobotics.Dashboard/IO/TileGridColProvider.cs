using Newtonsoft.Json.Serialization;
using StealthRobotics.Dashboard.API.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StealthRobotics.Dashboard.IO
{
    public class TileGridColProvider : IValueProvider
    {
        public object GetValue(object target)
        {
            if (target is DependencyObject d)
            {
                return TileGrid.GetColumn(d);
            }
            return 0;
        }

        public void SetValue(object target, object value)
        {
            if (target is DependencyObject d)
            {
                TileGrid.SetColumn(d, (int)value);
            }
        }
    }
}
