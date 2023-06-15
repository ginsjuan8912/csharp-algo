using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PropertyBinder.ChangeListener
{
    public class Trackable : IDisposable
    {
        public record ChangedProperty
        {
            public string Name { get; set; }
            public string OldValue { get; set; }
            public string NewValue { get; set; }
        }
       
        //This represent a dictionary of properties that got updated
        private Dictionary<string, object> originalValues;
        //Represent a list of the changed records
        private List<ChangedProperty> changedProperties;

        public Trackable()
        {
            originalValues = new();
            changedProperties = new();
        }

        /// <summary>
        /// Reads all the values from the object
        /// </summary>
        public void ListenForChanges()
        {
            // Get all properties of the target object
            PropertyInfo[] properties =  GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                            .Where(p => p.CanWrite && p.GetSetMethod() != null)
                                            .ToArray();

            // Store the initial values of the properties
            foreach (PropertyInfo property in properties)
            {
                object value = property?.GetValue(this);
                originalValues[property.Name] = value ?? null;
            }
        }
        /// <summary>
        /// Calculates if there in change in all of the properties
        /// </summary>
        private void CalculateChanges()
        {

            PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                          .Where(p => p.CanWrite && p.GetSetMethod() != null)
                                          .ToArray();

            // Store the initial values of the properties
            foreach (PropertyInfo property in properties)
            {
                // Check if the property has changed by comparing the current value with the original value
                object originalValue = originalValues.ContainsKey(property.Name) ? originalValues[property.Name] : string.Empty;
                object currentValue = property.GetValue(this);

                if (!Equals(originalValue, currentValue))
                {
                    changedProperties.Add(new ChangedProperty() { Name = property.Name, NewValue = currentValue?.ToString(), OldValue = originalValue?.ToString() });
                }
            }



        }

        public List<ChangedProperty> GetChanges()
        {
            CalculateChanges();
            return changedProperties;
        }
        public void Dispose()
        {
            changedProperties.Clear();
        }
    }
}
