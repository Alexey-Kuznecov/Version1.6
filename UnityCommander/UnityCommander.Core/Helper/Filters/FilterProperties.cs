
namespace UnityCommander.Core.Helper.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// The property not is null.
    /// </summary>
    public class PropertyNotIsNull
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; } 
    }

    /// <summary>
    /// The filter properties.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class FilterProperties<T>
    {
        /// <summary>
        /// The method is intended for properties that are null and are not a string type,
        /// the main purpose of the method is to output information in the UI properties containing the value,
        /// where Name is the name of the property and where Value is the value of this property.
        /// </summary>
        /// <param name="obj">Accepts the object in which you want to filter.</param>
        /// <returns>An object with fields that contain values</returns>
        public List<PropertyNotIsNull> Filter(T obj)
        {
            List<PropertyNotIsNull> field = new List<PropertyNotIsNull>();
            Type classType = typeof(T);
            PropertyInfo[] properties = classType.GetProperties();

            foreach (var property in properties)
            {
                if (property.GetValue(obj) != null && property.GetValue(obj) is string)
                {
                    field.Add(new PropertyNotIsNull
                    {
                       // Name = PackageFieldConverter.Dictionary.SingleOrDefault(k => k.Key == property.Name).Value,
                        Value = obj.GetType().GetProperty(property.Name)?.GetValue(obj).ToString()
                    });
                }
            }

            return field;
        }
    }
}
