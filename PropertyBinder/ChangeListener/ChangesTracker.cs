using System.Reflection;

namespace PropertyBinder.ChangeListener
{
    /// <summary>
    /// The following class uses reflection to 
    /// get all the properties and listen for 
    /// any change
    /// </summary>
    internal class ChangesTracker<T> where T : class
    {
        internal record ChangedProperty {
            public string Name { get; set; }
            public string OldValue { get; set; } 
            public string NewValue { get; set; }
        }

        //This represents the working item
        private T trackingInstance;
        //This represent a dictionary of properties that got updated
        private Dictionary<string, object> originalValues;
        //Represent a list of the changed records
        private List<ChangedProperty> changedProperties;

        public ChangesTracker(T trackerObj)
        {
            originalValues = new();
            changedProperties = new();
            trackingInstance = trackerObj;
            GetCurrentValues();
        }
    
        /// <summary>
        /// Reads all the values from the object
        /// </summary>
        private void GetCurrentValues()
        {           
            // Get all properties of the target object
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                            .Where(p => p.CanWrite && p.GetSetMethod() != null)
                                            .ToArray();

            // Store the initial values of the properties
            foreach (PropertyInfo property in properties)
            {
                object value = property?.GetValue(trackingInstance);
                originalValues[property.Name] = value ?? null;
            }
        }
        /// <summary>
        /// Calculates if there in change in all of the properties
        /// </summary>
        private void CalculateChanges()
        {

            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                          .Where(p => p.CanWrite && p.GetSetMethod() != null)
                                          .ToArray();

            // Store the initial values of the properties
            foreach (PropertyInfo property in properties)
            {
                // Check if the property has changed by comparing the current value with the original value
                object originalValue = originalValues.ContainsKey(property.Name) ? originalValues[property.Name] : string.Empty;
                object currentValue = property.GetValue(trackingInstance);

                if(!Equals(originalValue, currentValue))
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
    }
}
