using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Fux.Core.Attribute;

namespace Fux.Core
{
    /// <summary>
    /// This class is responsible for converting POCOs between one another
    /// </summary>
    public static class Convert
    {
        /// <summary>
        /// This delegate provides structure for a callback to retrieve the value of a property with loose typing
        /// </summary>
        /// <param name="attribute"></param>
        public delegate dynamic DelegateGetValueCallback(dynamic attribute);

        /// <summary>
        /// This delegate provides structure for a callback to retrieve the value of a property with strict typing
        /// </summary>
        /// <param name="attribute"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        public delegate TValue DelegateGetValueCallback<out TValue, in TAttribute>(TAttribute attribute)
            where TAttribute : FromPropertyAttribute;

        /// <summary>
        /// This delegate provides structure for a callback to retrieve the value of a property asynchronously
        /// with loose typing
        /// </summary>
        /// <param name="attribute"></param>
        public delegate Task<dynamic> DelegateGetValueCallbackAsync(dynamic attribute);

        /// <summary>
        /// This delegate provides structure for a callback to retrieve the value of a property asynchronously
        /// with strict typing
        /// </summary>
        /// <param name="attribute"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        public delegate Task<TValue> DelegateGetValueCallbackAsync<TValue, in TAttribute>(TAttribute attribute)
            where TAttribute : FromPropertyAttribute;

        /// <summary>
        /// This method provides all of the magic for converting <paramref name="sourceInstance"/> from
        /// an object of type <paramref name="sourceInstance"/> to a new object of type <paramref name="targetType"/>
        /// utilizing the From attribute in <paramref name="fromAttribute"/> and the To attribute in <paramref name="toAttribute"/>
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        /// <param name="sourceInstance"></param>
        /// <param name="fromAttribute"></param>
        /// <param name="toAttribute"></param>
        /// <returns></returns>
        private static dynamic convertObjectToTarget(Type sourceType, Type targetType, dynamic sourceInstance,
            Type fromAttribute, Type toAttribute)
        {
            // Check the from attribute type and default it
            if (fromAttribute == null) fromAttribute = typeof(FromPropertyAttribute);
            // Check the to attribute type and default it
            if (toAttribute == null) toAttribute = typeof(ToPropertyAttribute);
            // Reflect our source type
            Reflection<dynamic> sourceReflection = Reflection.Instantiate(sourceType);
            // Reflect our target type
            Reflection<dynamic> targetReflection = Reflection.Instantiate(targetType);
            // Localize our target instance
            dynamic targetInstance = targetReflection.Instance();
            // Flatten and normalize the source's properties
            Dictionary<string, PropertyInfo> flattenedSource = sourceReflection.FlattenAndNormalize();
            // Flatten and normalize the target's properties
            Dictionary<string, PropertyInfo> flattenedTarget = targetReflection.FlattenAndNormalize();
            // Iterate over the flattened target
            foreach (KeyValuePair<string, PropertyInfo> property in flattenedTarget)
            {
                // Localize the normalized name
                string normalizedName = property.Key;
                // Localize the property information
                PropertyInfo propertyInfo = property.Value;
                // Set the value on the property of the target instance
                propertyInfo.SetValue(targetInstance,
                    getPropertyValueForTarget(propertyInfo, sourceInstance, flattenedSource, toAttribute,
                        fromAttribute));
            }
            // We're done, return the populated target instance
            return targetInstance;
        }

        /// <summary>
        /// This method grabs the value for the target property from the source instance
        /// </summary>
        /// <param name="targetProperty"></param>
        /// <param name="sourceInstance"></param>
        /// <param name="flattenedSource"></param>
        /// <param name="toAttribute"></param>
        /// <param name="fromAttribute"></param>
        /// <returns></returns>
        private static dynamic getPropertyValueForTarget(PropertyInfo targetProperty, dynamic sourceInstance,
            Dictionary<string, PropertyInfo> flattenedSource, Type toAttribute, Type fromAttribute)
        {
            // Localize the normalized name of the property
            string normalizedTargetName =
                (targetProperty.GetCustomAttributes(fromAttribute).FirstOrDefault() as FromPropertyAttribute)?.Name;
            // Check for the normalized target name in the source and return the value
            // We do this because we favor the ToPropertyAttribute
            if (flattenedSource.ContainsKey(normalizedTargetName))
                return flattenedSource
                    .Where(s => s.Key.Equals(normalizedTargetName))
                    .Select(s => s.Value)
                    .FirstOrDefault()
                    ?.GetValue(sourceInstance);

            // Iterate over the properties in the flattened source
            foreach (KeyValuePair<string, PropertyInfo> property in flattenedSource)
            {
                // Localize the normalized name of the property
                string normalizedSourceName = property.Key;
                // Localize the property information
                PropertyInfo propertyInfo = property.Value;
                // Check for a to attribute and return the value
                if ((propertyInfo.GetCustomAttributes(toAttribute).FirstOrDefault() as ToPropertyAttribute).Name.Equals(
                    normalizedSourceName))
                    return propertyInfo.GetValue(sourceInstance);
                // Check the property name and set the value
                if (propertyInfo.Name.ToLower().Equals(targetProperty.Name.ToLower()))
                    return propertyInfo.GetValue(sourceInstance);
            }

            // We're done, we have nothing to return
            return null;
        }

        /// <summary>
        /// This method flattens and normalizes an object and executes a callback
        /// on each property to get its value
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="callback"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        private static dynamic generateObjectWithCallback(Type targetType, DelegateGetValueCallback callback,
            Type attributeType)
        {
            // Check the attribute type and default it
            if (attributeType == null) attributeType = typeof(FromPropertyAttribute);
            // Reflect our target type
            Reflection<dynamic> targetReflection = Reflection.Instantiate(targetType);
            // Localize the instance
            dynamic instance = targetReflection.Instance();
            // Flatten the object
            Dictionary<string, PropertyInfo> flattenedTarget = targetReflection.FlattenAndNormalize();
            // Iterate over the properties
            foreach (KeyValuePair<string, PropertyInfo> property in flattenedTarget)
            {
                // Localize the normalized property name
                string normalizedName = property.Key;
                // Localize the property information
                PropertyInfo propertyInfo = property.Value;
                // Check the property's attributes for our namespace
                foreach (System.Attribute attribute in propertyInfo.GetCustomAttributes())
                {
                    // Check the attribute's type
                    if (attribute.GetType() == attributeType)
                    {
                        // Localize the attribute
                        FromPropertyAttribute typedAttribute = (FromPropertyAttribute) attribute;
                        // Execute the callback and set the value on the property of the object
                        propertyInfo.SetValue(instance, callback.Invoke(typedAttribute));
                    }
                }
            }

            // We're done, return the new instance
            return instance;
        }

        /// <summary>
        /// This method asynchronously flattens and normalizes an object and executes a callback
        /// on each property to get its value
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="callback"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        private static async Task<dynamic> generateObjectWithCallbackAsync(Type targetType,
            DelegateGetValueCallbackAsync callback, Type attributeType)
        {
            // Check the attribute type and default it
            if (attributeType == null) attributeType = typeof(FromPropertyAttribute);
            // Reflect our target type
            Reflection<dynamic> targetReflection = Reflection.Instantiate(targetType);
            // Localize the instance
            dynamic instance = targetReflection.Instance();
            // Flatten the object
            Dictionary<string, PropertyInfo> flattenedTarget = targetReflection.FlattenAndNormalize();
            // Iterate over the properties
            foreach (KeyValuePair<string, PropertyInfo> property in flattenedTarget)
            {
                // Localize the normalized property name
                string normalizedName = property.Key;
                // Localize the property information
                PropertyInfo propertyInfo = property.Value;
                // Check the property's attributes for our namespace
                foreach (System.Attribute attribute in propertyInfo.GetCustomAttributes())
                {
                    // Check the attribute's type
                    if (attribute.GetType() == attributeType)
                    {
                        // Localize the attribute
                        FromPropertyAttribute typedAttribute = (FromPropertyAttribute) attribute;
                        // Execute the callback and set the value on the property of the object
                        propertyInfo.SetValue(instance, await callback.Invoke(typedAttribute));
                    }
                }
            }

            // We're done, return the new instance
            return instance;
        }

        /// <summary>
        /// This method converts <paramref name="sourceInstance"/> of type <paramref name="sourceType"/>
        /// to a new object of type <paramref name="targetType"/>
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        /// <param name="sourceInstance"></param>
        /// <param name="fromAttribute"></param>
        /// <param name="toAttribute"></param>
        /// <returns></returns>
        public static dynamic Map(Type sourceType, Type targetType, dynamic sourceInstance, Type fromAttribute = null,
            Type toAttribute = null) =>
            convertObjectToTarget(sourceType, targetType, sourceInstance, fromAttribute, toAttribute);

        /// <summary>
        /// This method converts <paramref name="sourceInstance"/> of type <typeparamref name="TFrom"/>
        /// to a new object of type <typeparamref name="TTo"/>
        /// </summary>
        /// <param name="sourceInstance"></param>
        /// <param name="fromAttribute"></param>
        /// <param name="toAttribute"></param>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <returns></returns>
        public static TTo Map<TFrom, TTo>(TFrom sourceInstance, Type fromAttribute = null, Type toAttribute = null) =>
            (TTo) convertObjectToTarget(typeof(TFrom), typeof(TTo), sourceInstance, fromAttribute, toAttribute);

        /// <summary>
        /// This method maps values to a new object instance from an external
        /// source using <paramref name="callback"/> with a loose return type
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="callback"></param>
        /// <param name="fromAttribute"></param>
        /// <returns></returns>
        public static dynamic MapWithValueGetter(Type targetType, DelegateGetValueCallback callback,
            Type fromAttribute = null) => generateObjectWithCallback(targetType, callback, fromAttribute);

        /// <summary>
        /// This method maps values to a new object instance <typeparamref name="TTarget"/> from an external
        /// source using <paramref name="callback"/> with a strict return type
        /// </summary>
        /// <param name="callback"></param>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        public static TTarget
            MapWithValueGetter<TTarget, TValue, TAttribute>(DelegateGetValueCallback<TValue, TAttribute> callback)
            where TAttribute : FromPropertyAttribute => generateObjectWithCallback(typeof(TTarget),
            (callback as DelegateGetValueCallback), typeof(TAttribute));

        /// <summary>
        /// This method asynchronously maps values to a new object instance from an external
        /// source using <paramref name="callback"/> with a loose return type
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="callback"></param>
        /// <param name="fromAttribute"></param>
        /// <returns></returns>
        public static Task<dynamic> MapWithValueGetterAsync(Type targetType, DelegateGetValueCallbackAsync callback,
            Type fromAttribute = null) => generateObjectWithCallbackAsync(targetType, callback, fromAttribute);

        /// <summary>
        /// This method asynchronously maps values to a new object instance <typeparamref name="TTarget"/> from an external
        /// source using <paramref name="callback"/> with a strict return type 
        /// </summary>
        /// <param name="callback"></param>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        public static Task<TTarget>
            MapWithValueGetterAsync<TTarget, TValue, TAttribute>(
                DelegateGetValueCallbackAsync<TValue, TAttribute> callback) where TAttribute : FromPropertyAttribute =>
            (generateObjectWithCallbackAsync(typeof(TTarget), (callback as DelegateGetValueCallbackAsync),
                typeof(TAttribute)) as Task<TTarget>);
    }

    /// <summary>
    /// This class provides a generic for convertible objects
    /// </summary>
    /// <typeparam name="TFrom"></typeparam>
    public static class Convert<TFrom>
    {
        /// <summary>
        /// This method maps <typeparamref name="TFrom"/> <paramref name="sourceInstance"/> to a
        /// new instance of <typeparamref name="TTo"/>
        /// </summary>
        /// <param name="sourceInstance"></param>
        /// <param name="fromAttribute"></param>
        /// <param name="toAttribute"></param>
        /// <returns></returns>
        /// <typeparam name="TTo"></typeparam>
        public static TTo Map<TTo>(TFrom sourceInstance, Type fromAttribute = null, Type toAttribute = null) =>
            Convert.Map<TFrom, TTo>(sourceInstance, fromAttribute, toAttribute);
    }

    /// <summary>
    /// This class provides a generic for convertible objects
    /// </summary>
    /// <typeparam name="TFrom"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    public static class Convert<TFrom, TTo>
    {
        /// <summary>
        /// This method maps <typeparamref name="TFrom"/> <paramref name="sourceInstance"/> to a
        /// new instance of <typeparamref name="TTo"/>
        /// </summary>
        /// <param name="sourceInstance"></param>
        /// <param name="fromAttribute"></param>
        /// <param name="toAttribute"></param>
        /// <returns></returns>
        public static TTo Map(TFrom sourceInstance, Type fromAttribute = null, Type toAttribute = null) =>
            Convert.Map<TFrom, TTo>(sourceInstance, fromAttribute, toAttribute);
    }
}