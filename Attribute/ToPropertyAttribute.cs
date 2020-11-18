using System;

namespace Fux.Core.Attribute
{
    /// <summary>
    /// This attribute maintains the correlation between properties when mapping to other objects
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ToPropertyAttribute : System.Attribute
    {
        /// <summary>
        /// This property contains the name of the property on the target object to map to
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// This method instantiates the attribute with a <paramref name="propertyName"/> to map to
        /// </summary>
        /// <param name="propertyName"></param>
        public ToPropertyAttribute(string propertyName) => Name = propertyName;
    }
}
