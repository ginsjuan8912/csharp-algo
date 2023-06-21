using System;
using System.Collections.Generic;
using System.Dynamic;

namespace ginsjuan.Extensions
{
    public class ConfigurationReader
    {
        /// <summary>
        /// Read a connection string a transform it into an dynamic dictionary
        /// </summary>
        /// <param name="connectionString">The connection string to read and transform</param>
        /// <returns>A dictionary with the keys and values returned from the connection string</returns>
        public static dynamic ReadConnectionString(string connectionString)
        {
            dynamic dynamicObject = new ExpandoObject();
            var refString = Environment.GetEnvironmentVariable(connectionString);

            if (refString != null)
            {
                var keyValue = refString.Split(";");
                foreach (var items in keyValue)
                {
                    var key = items.Substring(0,items.IndexOf("="));
                    var value = items.Substring(items.IndexOf("=") + 1, items.Length - key.Length - 1);
                    if(!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                    {
                        ((IDictionary<string, object>)dynamicObject).Add(key, value);
                    }
                }
            }

            return dynamicObject;
        }
    }
}