using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StealthRobotics.Dashboard.API
{
    internal static class Util
    {
        public static string ToTitleCase(string str)
        {
            return Regex.Replace(str, @"[a-zA-Z]([A-Z]|\d)", m => $"{m.Value[0]} {m.Value[1]}");
        }

        public static string ToCamelCase(string str)
        {
            return str.Replace(" ", "");
        }
    }
}
