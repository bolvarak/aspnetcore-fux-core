namespace Fux.Core.Attribute
{
    /// <summary>
    /// This attribute maintains the correlation between properties when mapping from other objects 
    /// </summary>
    public class FromKeyAttribute : FromPropertyAttribute
    {
        /// <summary>
        /// This method instantiates our attribute with a key name
        /// </summary>
        /// <param name="keyName"></param>
        public FromKeyAttribute(string keyName) : base(keyName) { }
    }
}