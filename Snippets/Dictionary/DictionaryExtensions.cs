using System;
using System.Collections.Generic;

namespace ginsjuan.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Perform a cast operation to convert the value from the
        /// dictionary to the specified primitive type.
        /// </summary>
        /// <typeparam Name="T">Primitive Type to convert</typeparam>
        /// <param Name="dictionary"></param>
        /// <param Name="key">Key that is going to be restored an casted from the dictionary</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throws a ArgumentNullException if the dictionary is null</exception>
        /// <exception cref="InvalidCastException">Throws an InvalidCastException if the cast cannot be performed</exception>
        /// <exception cref="KeyNotFoundException">Throws a KeyNotFoundException if the key is not present in the dictionary</exception>
        public static T GetValueAs<T>(this Dictionary<string, object> dictionary, string key)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if (dictionary.ContainsKey(key))
            {
                var value = dictionary[key];

                if (typeof(T).IsPrimitive && !string.IsNullOrEmpty(value.ToString()))
                {

                    var castedValue = (T) Convert.ChangeType(value, typeof(T));

                    return castedValue;
                }
                else
                {
                    throw new InvalidCastException($"Unable to cast to {typeof(T).Name}");
                }
            }
            else
            {
                // Handle the case when the key doesn't exist in the dictionary.
                throw new KeyNotFoundException($"Key '{key}' does not exist in the dictionary.");
            }
        }
    }
}
