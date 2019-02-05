﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace StealthRobotics.Dashboard.API.Network
{
    /// <summary>
    /// This is a dictionary guaranteed to have only one of each value and key. 
    /// It may be searched either by TFirst or by TSecond, giving a unique answer because it is 1 to 1.
    /// </summary>
    /// <typeparam name="TFirst">The type of the "key"</typeparam>
    /// <typeparam name="TSecond">The type of the "value"</typeparam>
    public class OneToOneConversionMap<TFirst, TSecond>
    {
        IDictionary<TFirst, TSecond> firstToSecond = new Dictionary<TFirst, TSecond>();
        IDictionary<TSecond, TFirst> secondToFirst = new Dictionary<TSecond, TFirst>();
        IDictionary<TFirst, IValueConverter> conversionMap = new Dictionary<TFirst, IValueConverter>();

        #region Exception throwing methods

        /// <summary>
        /// Tries to add the pair to the dictionary.
        /// Throws an exception if either element is already in the dictionary
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public void Add(TFirst first, TSecond second)
        {
            if (firstToSecond.ContainsKey(first) || secondToFirst.ContainsKey(second))
                throw new ArgumentException("Duplicate first or second");

            firstToSecond.Add(first, second);
            secondToFirst.Add(second, first);
            conversionMap.Add(first, null);
        }

        /// <summary>
        /// Find the TSecond corresponding to the TFirst first
        /// Throws an exception if first is not in the dictionary.
        /// </summary>
        /// <param name="first">the key to search for</param>
        /// <returns>the value corresponding to first</returns>
        public TSecond GetByFirst(TFirst first)
        {
            if (!firstToSecond.TryGetValue(first, out TSecond second))
                throw new ArgumentException("first");

            return second;
        }

        /// <summary>
        /// Find the TFirst corresponing to the Second second.
        /// Throws an exception if second is not in the dictionary.
        /// </summary>
        /// <param name="second">the key to search for</param>
        /// <returns>the value corresponding to second</returns>
        public TFirst GetBySecond(TSecond second)
        {
            if (!secondToFirst.TryGetValue(second, out TFirst first))
                throw new ArgumentException("second");

            return first;
        }


        /// <summary>
        /// Remove the record containing first.
        /// If first is not in the dictionary, throws an Exception.
        /// </summary>
        /// <param name="first">the key of the record to delete</param>
        public void RemoveByFirst(TFirst first)
        {
            if (!firstToSecond.TryGetValue(first, out TSecond second))
                throw new ArgumentException("first");

            firstToSecond.Remove(first);
            secondToFirst.Remove(second);
            conversionMap.Remove(first);
        }

        /// <summary>
        /// Remove the record containing second.
        /// If second is not in the dictionary, throws an Exception.
        /// </summary>
        /// <param name="second">the key of the record to delete</param>
        public void RemoveBySecond(TSecond second)
        {
            if (!secondToFirst.TryGetValue(second, out TFirst first))
                throw new ArgumentException("second");

            secondToFirst.Remove(second);
            firstToSecond.Remove(first);
            conversionMap.Remove(first);
        }

        /// <summary>
        /// Maps in a type converter to a keypair containing first
        /// Throws an exception if first is not in the dictionary
        /// </summary>
        /// <param name="first">The key to search for</param>
        /// <param name="converter">The converter to map</param>
        public void MapConversionByFirst(TFirst first, IValueConverter converter)
        {
            if (!firstToSecond.TryGetValue(first, out TSecond second))
                throw new ArgumentException("first");

            conversionMap.Add(first, converter);
        }

        /// <summary>
        /// Maps in a type converter to a keypair containing second
        /// Throws an exception if second is not in the dictionary
        /// </summary>
        /// <param name="second">The key to search for</param>
        /// <param name="converter">The converter to map</param>
        public void MapConversionBySecond(TSecond second, IValueConverter converter)
        {
            if (!secondToFirst.TryGetValue(second, out TFirst first))
                throw new ArgumentException("second");

            conversionMap.Add(first, converter);
        }

        /// <summary>
        /// Gets the type converter corresponding the the keypair containing first
        /// Throws an exception if first is not in the dictionary
        /// </summary>
        /// <param name="first">The key to search for</param>
        public IValueConverter GetConverterByFirst(TFirst first)
        {
            if (!firstToSecond.TryGetValue(first, out TSecond second))
                throw new ArgumentException("first");

            return conversionMap[first];
        }

        /// <summary>
        /// Gets the type converter corresponding the the keypair containing second
        /// Throws an exception if second is not in the dictionary
        /// </summary>
        /// <param name="second">The key to search for</param>
        public IValueConverter GetConverterBySecond(TSecond second)
        {
            if (!secondToFirst.TryGetValue(second, out TFirst first))
                throw new ArgumentException("second");

            return conversionMap[first];
        }

        #endregion

        #region Try methods

        /// <summary>
        /// Tries to add the pair to the dictionary.
        /// Returns false if either element is already in the dictionary        
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns>true if successfully added, false if either element is already in the dictionary</returns>
        public bool TryAdd(TFirst first, TSecond second)
        {
            if (firstToSecond.ContainsKey(first) || secondToFirst.ContainsKey(second))
                return false;

            firstToSecond.Add(first, second);
            secondToFirst.Add(second, first);
            return true;
        }


        /// <summary>
        /// Find the TSecond corresponding to the TFirst first.
        /// Returns false if first is not in the dictionary.
        /// </summary>
        /// <param name="first">the key to search for</param>
        /// <param name="second">the corresponding value</param>
        /// <returns>true if first is in the dictionary, false otherwise</returns>
        public bool TryGetByFirst(TFirst first, out TSecond second)
        {
            return firstToSecond.TryGetValue(first, out second);
        }

        /// <summary>
        /// Find the TFirst corresponding to the TSecond second.
        /// Returns false if second is not in the dictionary.
        /// </summary>
        /// <param name="second">the key to search for</param>
        /// <param name="first">the corresponding value</param>
        /// <returns>true if second is in the dictionary, false otherwise</returns>
        public bool TryGetBySecond(TSecond second, out TFirst first)
        {
            return secondToFirst.TryGetValue(second, out first);
        }

        /// <summary>
        /// Remove the record containing first, if there is one.
        /// </summary>
        /// <param name="first"></param>
        /// <returns> If first is not in the dictionary, returns false, otherwise true</returns>
        public bool TryRemoveByFirst(TFirst first)
        {
            if (!firstToSecond.TryGetValue(first, out TSecond second))
                return false;

            firstToSecond.Remove(first);
            secondToFirst.Remove(second);
            conversionMap.Remove(first);
            return true;
        }

        /// <summary>
        /// Remove the record containing second, if there is one.
        /// </summary>
        /// <param name="second"></param>
        /// <returns> If second is not in the dictionary, returns false, otherwise true</returns>
        public bool TryRemoveBySecond(TSecond second)
        {
            if (!secondToFirst.TryGetValue(second, out TFirst first))
                return false;

            secondToFirst.Remove(second);
            firstToSecond.Remove(first);
            conversionMap.Remove(first);
            return true;
        }

        #endregion

        public TFirst this[TSecond second]
        {
            get
            {
                return GetBySecond(second);
            }
        }

        public TSecond this[TFirst first]
        {
            get
            {
                return GetByFirst(first);
            }
        }

        /// <summary>
        /// The number of pairs stored in the dictionary
        /// </summary>
        public int Count
        {
            get { return firstToSecond.Count; }
        }

        /// <summary>
        /// Removes all items from the dictionary.
        /// </summary>
        public void Clear()
        {
            firstToSecond.Clear();
            secondToFirst.Clear();
        }
    }
}
