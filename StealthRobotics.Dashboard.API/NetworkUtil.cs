using NetworkTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealthRobotics.Dashboard.API
{
    public static class NetworkUtil
    {
        /// <summary>
        /// The SmartDashboard network table path
        /// </summary>
        public static NetworkTable SmartDashboard
        {
            get
            {
                return NetworkTable.GetTable("/SmartDashboard");
            }
        }

        /// <summary>
        /// Gets the .NET type of a NetworkTable simple value
        /// </summary>
        /// <param name="v">The value to check</param>
        public static Type TypeOf(Value v)
        {
            switch (v?.Type)
            {
                case NtType.Boolean:
                    return typeof(bool);
                case NtType.BooleanArray:
                    return typeof(bool[]);
                case NtType.Double:
                    return typeof(double);
                case NtType.DoubleArray:
                    return typeof(double[]);
                case NtType.String:
                    return typeof(string);
                case NtType.StringArray:
                    return typeof(string[]);
                case NtType.Raw:
                    return typeof(byte[]);
                case NtType.Rpc:
                    //remote procedure call, should not be trying to handle these
                    throw new InvalidCastException("Can't handle bindings to remote procedure calls");
                default:
                    return null;
            }
        }

        /// <summary>
        /// Converts the NetworkTable simple value into an actual .NET data value
        /// </summary>
        /// <param name="v">The value to convert</param>
        public static object ReadValue(Value v)
        {
            switch (v?.Type)
            {
                case NtType.Boolean:
                    return v.GetBoolean();
                case NtType.BooleanArray:
                    return v.GetBooleanArray();
                case NtType.Double:
                    return v.GetDouble();
                case NtType.DoubleArray:
                    return v.GetDoubleArray();
                case NtType.String:
                    return v.GetString();
                case NtType.StringArray:
                    return v.GetStringArray();
                default:
                    return null;
            }
        }
    }
}
