using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace StealthRobotics.Dashboard.API.Network
{
    /// <summary>
    /// A base type for a conversion mapping between 2 types across the network
    /// </summary>
    /// <typeparam name="TLocal">The type on the local dashboard</typeparam>
    /// <typeparam name="TNetwork">The type on the network table</typeparam>
    public abstract class NTConverter<TLocal, TNetwork> : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is TLocal)
            {
                return ConvertToNetwork((TLocal)value);
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is TNetwork)
            {
                return ConvertToLocal((TNetwork)value);
            }
            return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// A function that maps a local value to a network value
        /// </summary>
        /// <param name="val">The local value being converted</param>
        public abstract TNetwork ConvertToNetwork(TLocal val);
        /// <summary>
        /// A function that maps a network value to a local value
        /// </summary>
        /// <param name="val">The network value being converted</param>
        /// <returns></returns>
        public abstract TLocal ConvertToLocal(TNetwork val);
    }
}
