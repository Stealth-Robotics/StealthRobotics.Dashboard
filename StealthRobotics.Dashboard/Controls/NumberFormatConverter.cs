using StealthRobotics.Dashboard.API.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealthRobotics.Dashboard.Controls
{
    class NumberFormatConverter : NTConverter<string, double>
    {
        public NumberFormat Format { get; set; } = NumberFormat.General;

        public int Precision { get; set; } = 3;

        public override string ConvertToLocal(double val)
        {
            return val.ToString($"{(char)Format}{Precision}");
        }

        public override double ConvertToNetwork(string val)
        {
            if(Format == NumberFormat.Percent)
            {
                //make sure we can even parse
                val = val.Replace("%", "");
            }
            if(double.TryParse(val, out double ret))
            {
                if(Format == NumberFormat.Percent)
                {
                    ret /= 100;
                }
                return ret;
            }
            return double.NaN;
        }
    }
}
