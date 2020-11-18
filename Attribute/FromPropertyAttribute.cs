using System;

namespace Fux.Core.Attribute
{
    /// <summary>
    /// This attribute maintains the correlation between properties when mapping from other objects
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class FromPropertyAttribute : System.Attribute
    {
        /// <summary>
        /// This property contains the name of the property on the target object to map from
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// This method instantiates the attribute with a <paramref name="propertyName"/> to map from
        /// </summary>
        /// <param name="propertyName"></param>
        public FromPropertyAttribute(string propertyName) => Name = propertyName;
    }
}
