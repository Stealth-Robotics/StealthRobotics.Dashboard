using StealthRobotics.Dashboard.API.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealthRobotics.Dashboard.SamplePlugin
{
    //Convert between double[] on the local side and string on the robot side
    public class PowerUpFieldDataConverter : NTConverter<double[], string>
    {
        public override double[] ConvertToLocal(string val)
        {
            //only deal with correct data strings
            if(val.Length == 3)
            {
                double[] ret = new double[3];
                for(int i = 0; i < 3; i++)
                {
                    char pos = char.ToUpper(val[i]);
                    //if the position is right, scale to flip
                    //if the position is anything else including invalid value leave it be
                    ret[i] = (pos == 'R') ? -1 : 1;
                }
                return ret;
            }
            return new double[] { 1, 1, 1 };
        }

        public override string ConvertToNetwork(double[] val)
        {
            string ret = "";
            //we know this should only be length 3
            for(int i = 0; i < 3; i++)
            {
                double scale = val[i];
                //if we're flipped it's on the right. otherwise on the left
                ret += (scale == -1) ? "R" : "L";
            }
            return ret;
        }
    }
}
