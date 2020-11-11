using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Fux.Core
{
    /// <summary>
    /// This class provides an interface for fluidly instantiating objects
    /// </summary>
    public static class Reflection
    {
        /// <summary>
        /// This method flattens an object and normalizes the property names to outer<paramref name="separator"/>inner
        /// NOTE:  This uses case insensitivity, keep that in mind when using this against POCOs
        /// </summary>
        /// <param name="type"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static Dictionary<string, PropertyInfo> FlattenAndNormalize(Type type, char separator = '.') =>
            Instantiate(type).FlattenAndNormalize(separator);

        /// <summary>
        /// This method flattens a nested object and normalizes the property names
        /// to outer<paramref name="separator"/>inner
        /// NOTE:  This uses case insensitivity, keep that in mind when using this against POCOs
        /// </summary>
        /// <param name="type"></param>
        /// <param name="separator"></param>
        /// <param name="path"></param>
        /// <param name="flattenedProperties"></param>
        /// <returns></returns>
        public static Dictionary<string, PropertyInfo> FlattenAndNormalize(Type type, char separator, string path,
            ref Dictionary<string, PropertyInfo> flattenedProperties) =>
            Instantiate(type).FlattenAndNormalize(type, separator, path, ref flattenedProperties);

        /// <summary>
        /// This method flattens an object and normalizes the property names to outer<paramref name="separator"/>inner
        /// NOTE:  This uses case insensitivity, keep that in mind when using this against POCOs
        /// </summary>
        /// <param name="type"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static Dictionary<string, PropertyInfo> FlattenAndNormalize(Type type, string separator) =>
            FlattenAndNormalize(type, separator.ToCharArray().First());

        /// <summary>
        /// This method flattens an object and normalizes the property names to outer<paramref name="separator"/>inner
        /// NOTE:  This uses case insensitivity, keep that in mind when using this against POCOs
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        /// <typeparam name="TSource"></typeparam>
        public static Dictionary<string, PropertyInfo> FlattenAndNormalize<TSource>(char separator = '.') =>
            FlattenAndNormalize(typeof(TSource), separator);

        /// <summary>
        /// This method flattens an object and normalizes the property names to outer<paramref name="separator"/>inner
        /// NOTE:  This uses case insensitivity, keep that in mind when using this against POCOs
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        /// <typeparam name="TSource"></typeparam>
        public static Dictionary<string, PropertyInfo> FlattenAndNormalize<TSource>(string separator) =>
            FlattenAndNormalize(typeof(TSource), separator.ToCharArray().First());

        /// <summary>
        /// This method returns a property's value from its name
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static dynamic Get(Type type, string propertyName) =>
            Instantiate(type).Get<dynamic>(propertyName);

        /// <summary>
        /// This method returns the property's value from a lambda expression selector
        /// </summary>
        /// <param name="type"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static dynamic Get(Type type, Expression<Func<dynamic, dynamic>> selector) =>
            Instantiate(type).Get<dynamic>(selector);

        /// <summary>
        /// This method fluidly instantiates an object with constructor arguments
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static dynamic Instance(Type type) =>
            Instantiate(type).Instance();

        /// <summary>
        /// This method fluidly instantiates an object with constructor arguments
        /// </summary>
        /// <param name="type"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static dynamic Instance(Type type, IEnumerable<object> arguments) =>
            Instantiate(type, arguments).Instance();

        /// <summary>
        /// This method fluidly instantiates an object with constructor arguments
        /// </summary>
        /// <param name="type"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static dynamic Instance(Type type, params object[] arguments) =>
            Instantiate(type, arguments).Instance();

        /// <summary>
        /// This method fluidly instantiates an object from its system type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Reflection<dynamic> Instantiate(Type type) =>
            (Reflection<dynamic>)Activator.CreateInstance(typeof(Reflection<>).MakeGenericType(new[] { type }));

        /// <summary>
        /// This method fluidly instantiates an object with constructor arguments from its system type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static Reflection<dynamic> Instantiate(Type type, IEnumerable<object> arguments) =>
            (Reflection<dynamic>)Activator.CreateInstance(typeof(Reflection<>).MakeGenericType(new[] { type }), arguments);

        /// <summary>
        /// This method fluidly instantiates an object with constructor arguments from its system type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static Reflection<dynamic> Instantiate(Type type, params object[] arguments) =>
            Instantiate(type, arguments);

        /// <summary>
        /// This method calls a method on the instantiated object
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public static dynamic Invoke(Type type, string methodName) =>
            Instantiate(type).Invoke<dynamic>(methodName);

        /// <summary>
        /// This method calls a method with arguments on the instantiated object
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static dynamic Invoke(Type type, string methodName, IEnumerable<object> arguments) =>
            Instantiate(type).Invoke<dynamic>(methodName, arguments);

        /// <summary>
        /// This method calls a method with arguments on the instantiated object
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static dynamic Invoke(Type type, string methodName, params object[] arguments) =>
            Instantiate(type).Invoke<dynamic>(methodName, arguments);

        /// <summary>
        /// This method calls a generic method on the instantiated object
        /// </summary>
        /// <param name="classType"></param>
        /// <param name="methodType"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public static dynamic InvokeGeneric(Type classType, Type methodType, string methodName) =>
            Instantiate(classType).InvokeGeneric<dynamic>(methodType, methodName);

        /// <summary>
        /// This method calls a generic method with arguments on the object
        /// </summary>
        /// <param name="classType"></param>
        /// <param name="methodType"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static dynamic InvokeGeneric(Type classType, Type methodType, string methodName,
            IEnumerable<object> arguments) =>
            Instantiate(classType).InvokeGeneric<dynamic>(methodType, methodName, arguments);

        /// <summary>
        /// This method calls a generic method with arguments on the instantiated object
        /// </summary>
        /// <param name="classType"></param>
        /// <param name="methodType"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static dynamic InvokeGeneric(Type classType, Type methodType, string methodName,
            params object[] arguments) =>
            Instantiate(classType).InvokeGeneric<dynamic>(methodType, methodName, arguments);

        /// <summary>
        /// This method returns the property information for a property
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static PropertyInfo PropertyInfo(Type type, string propertyName) =>
            Instantiate(type).PropertyInfo(propertyName);

        /// <summary>
        /// This method returns the property information structure for a property from a lambda expression selector
        /// </summary>
        /// <param name="type"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static PropertyInfo PropertyInfo(Type type, Expression<Func<dynamic, dynamic>> selector) =>
            Instantiate(type).PropertyInfo(selector);

        /// <summary>
        /// This method returns the property name from a lambda selector expression
        /// </summary>
        /// <param name="type"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static string PropertyName(Type type, Expression<Func<dynamic, dynamic>> selector) =>
            Instantiate(type).PropertyName(selector);

        /// <summary>
        /// This method sets the value or property <paramref name="propertyName"/>
        /// to <paramref name="value"/> on the instantiated object
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Reflection<dynamic> Set(Type type, string propertyName, dynamic value) =>
            Instantiate(type).Set<dynamic>(propertyName, value);

        /// <summary>
        /// This method sets the value of the property found with <paramref name="selector"/>
        /// to <paramref name="value"/> on the instantiated object
        /// </summary>
        /// <param name="type"></param>
        /// <param name="selector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Reflection<dynamic> Set(Type type, Expression<Func<dynamic, dynamic>> selector, dynamic value) =>
            Instantiate(type).Set<dynamic>(selector, value);
    }

    /// <summary>
    /// This class provides object reflection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Reflection<T>
    {
        /// <summary>
        /// This property contains the list of arguments to pass to the constructor
        /// </summary>
        private readonly List<object> _arguments = new List<object>();

        /// <summary>
        /// This property contains the instance of our reflection
        /// </summary>
        private T _instance;

        /// <summary>
        /// This property contains the PropertyInfo objects for the properties in <typeparamref name="T"/>
        /// </summary>
        private readonly Dictionary<string, PropertyInfo> _properties = new Dictionary<string, PropertyInfo>(
            typeof(T).GetProperties().Select(p => new KeyValuePair<string, PropertyInfo>(p.Name.ToLower(), p)));

        /// <summary>
        /// This property contains our property selector lambda expressions
        /// </summary>
        private readonly Dictionary<Expression<Func<T, dynamic>>, PropertyInfo> _propertySelectors =
            new Dictionary<Expression<Func<T, dynamic>>, PropertyInfo>();

        /// <summary>
        /// This property contains the MethodInfo objects for the properties in <typeparamref name="T"/>
        /// </summary>
        private readonly Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>(
            typeof(T).GetMethods().Select(m => new KeyValuePair<string, MethodInfo>(m.Name.ToLower(), m)));

        /// <summary>
        /// This property contains the type in which we will be reflecting
        /// </summary>
        private readonly Type _type = typeof(T);

        /// <summary>
        /// This method reflects and instantiates the object
        /// </summary>
        public Reflection() => Instantiate();

        /// <summary>
        /// This method reflects and instantiates the object with constructor arguments
        /// </summary>
        /// <param name="arguments"></param>
        public Reflection(IEnumerable<object> arguments) => WithArguments(arguments).Instantiate();

        /// <summary>
        /// This method reflects and instantiates the object with constructor arguments 
        /// </summary>
        /// <param name="arguments"></param>
        public Reflection(params object[] arguments) => WithArguments(arguments).Instantiate();

        /// <summary>
        /// This method localizes the property information from a lambda selector expressions
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        protected PropertyInfo ExpressionToPropertyInfo(Expression<Func<T, dynamic>> selector)
        {
            // Check to see if the instance has processed this reflection before
            if (!_propertySelectors.Any(e => e.Key.Equals(selector)))
            {
                // Localize the member expression
                MemberExpression memberExpression = (selector.Body as MemberExpression);
                // Make sure we have a member expression
                if (memberExpression != null)
                {
                    // Localize the property information
                    PropertyInfo property = (memberExpression.Member as PropertyInfo);
                    // Make sure we have property information and set the value into the instance
                    if (property != null) _propertySelectors[selector] = property;
                }
            }

            // We're done, return the property info object
            return _propertySelectors
                .Where(e => e.Key.Equals(selector))
                .Select(e => e.Value)
                .FirstOrDefault();
        }

        /// <summary>
        /// This method invokes both normal and generic methods on objects using reflection
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <param name="genericType"></param>
        /// <returns></returns>
        protected TValue InvokeMethod<TValue>(T instance, string methodName, IEnumerable<object> arguments = null,
            Type genericType = null)
        {
            // Localize our method information
            MethodInfo methodInfo = _methods
                .Where(m => m.Key.Equals(methodName.ToLower()))
                .Select(m => m.Value)
                .FirstOrDefault();
            // Make sure we have a method
            if (methodInfo == null) return default;
            // Check for a generic and reset the information
            if (methodInfo.IsGenericMethod && !genericType.Equals(null))
                methodInfo = methodInfo.MakeGenericMethod(genericType);
            // Check for arguments and return the invocation
            if (arguments != null && arguments.Any()) return (TValue)methodInfo.Invoke(instance, arguments.ToArray());
            // We're done, return the invocation
            return (TValue)methodInfo.Invoke(instance, null);
        }

        /// <summary>
        /// This method sets the value of property <paramref name="propertyName"/>
        /// on <paramref name="instance"/> to <paramref name="value"/> of type <typeparamref name="TValue"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <typeparam name="TValue"></typeparam>
        protected void SetPropertyValue<TValue>(T instance, string propertyName, TValue value) =>
            PropertyInfo(propertyName)?.SetValue(instance, value);

        /// <summary>
        /// This method sets the value of the property found with <paramref name="expression"/>
        /// on <paramref name="instance"/> to <paramref name="value"/> of type <typeparamref name="TValue"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        /// <typeparam name="TValue"></typeparam>
        protected void SetPropertyValue<TValue>(T instance, Expression<Func<T, TValue>> expression, TValue value) =>
            PropertyInfo(expression)?.SetValue(instance, value);
        
        /// <summary>
        /// This method flattens an object and normalizes the property names to outer<paramref name="separator"/>inner
        /// NOTE:  This uses case insensitivity, keep that in mind when using this against POCOs
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public Dictionary<string, PropertyInfo> FlattenAndNormalize(char separator = '.')
        {
            // Define our properties
            Dictionary<string, PropertyInfo> flattenedProperties = new Dictionary<string, PropertyInfo>();
            // Iterate over the properties
            foreach (KeyValuePair<string, PropertyInfo> propertyInfo in Properties())
            {
                // Define the property path
                string path = propertyInfo.Value.Name.ToLower();
                // Check the property for a class and flatten it
                if (propertyInfo.Value.GetType().IsClass)
                    FlattenAndNormalize(propertyInfo.Value.GetType(), separator, path, ref flattenedProperties);
                // Add the property to the response
                flattenedProperties.Add(path, propertyInfo.Value);
            }
            // We're done, return the flattened object
            return flattenedProperties;
        }

        /// <summary>
        /// This method flattens a nested object and normalizes the property names
        /// to outer<paramref name="separator"/>inner
        /// NOTE:  This uses case insensitivity, keep that in mind when using this against POCOs
        /// </summary>
        /// <param name="type"></param>
        /// <param name="separator"></param>
        /// <param name="path"></param>
        /// <param name="flattenedProperties"></param>
        /// <returns></returns>
        public Dictionary<string, PropertyInfo> FlattenAndNormalize(Type type, char separator, string path,
            ref Dictionary<string, PropertyInfo> flattenedProperties)
        {
            // Reflect the type
            Reflection<dynamic> instance = Reflection.Instantiate(type);
            // Iterate over the properties
            foreach (KeyValuePair<string, PropertyInfo> propertyInfo in instance.Properties())
            {
                // Reset the property path
                path = $"{path}{separator.ToString()}{propertyInfo.Value.Name.ToLower()}";
                // Check the property for a class and flatten it
                if (propertyInfo.Value.GetType().IsClass)
                    instance.FlattenAndNormalize(propertyInfo.Value.GetType(), separator, path,
                        ref flattenedProperties);
                // Add the property to the response
                flattenedProperties.Add(path, propertyInfo.Value);
            }

            // We're done, return the flattened object
            return flattenedProperties;
        }

        /// <summary>
        /// This method flattens an object and normalizes the property names to outer<paramref name="separator"/>inner
        /// NOTE:  This uses case insensitivity, keep that in mind when using this against POCOs
        /// </summary>
        /// <param name="type"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public Dictionary<string, PropertyInfo> FlattenAndNormalize(Type type, string separator) =>
            FlattenAndNormalize(separator.ToCharArray().First());

        /// <summary>
        /// This method returns the property value from the property's name and provided instance
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public TValue Get<TValue>(T instance, string propertyName) =>
            (TValue)PropertyInfo(propertyName)?.GetValue(instance);

        /// <summary>
        /// This method returns the property value from a lambda expression selector and provided instance
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public TValue Get<TValue>(T instance, Expression<Func<T, dynamic>> selector) =>
            (TValue)PropertyInfo(selector)?.GetValue(instance);

        /// <summary>
        /// This method returns the property value from a lambda expression selector and provided instance
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="selector"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue Get<TValue>(T instance, Expression<Func<T, TValue>> selector) =>
            (TValue)PropertyInfo(selector)?.GetValue(instance);

        /// <summary>
        /// This method returns the property value from the property's name and existing instance
        /// </summary>
        /// <param name="propertyName"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue Get<TValue>(string propertyName) =>
            Get<TValue>(_instance, propertyName);

        /// <summary>
        /// This method returns the property value from a lambda expression selector and existing instance
        /// </summary>
        /// <param name="selector"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue Get<TValue>(Expression<Func<T, TValue>> selector) =>
            (TValue)Get(_instance, selector as Expression<Func<T, dynamic>>);

        /// <summary>
        /// This method instantiates an instance of our reflection
        /// </summary>
        /// <returns></returns>
        public T Instance()
        {
            // Make sure the object is instantiated
            Instantiate();
            // We're done, return the instance of the object
            return _instance;
        }

        /// <summary>
        /// This method generates an instance of the object
        /// </summary>
        /// <returns></returns>
        public Reflection<T> Instantiate()
        {
            // Check for an instance and instantiate the object
            if (_instance == null) _instance = Reflection.Instance(typeof(T));
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method invokes the method <paramref name="methodName"/>
        /// and returning <typeparamref name="TValue"/> on <paramref name="instance"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="methodName"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue Invoke<TValue>(T instance, string methodName) =>
            InvokeMethod<TValue>(instance, methodName);

        /// <summary>
        /// This method invokes the method <paramref name="methodName"/> with <paramref name="arguments"/>
        /// and returning <typeparamref name="TValue"/> on <paramref name="instance"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue Invoke<TValue>(T instance, string methodName, IEnumerable<object> arguments) =>
            InvokeMethod<TValue>(instance, methodName, arguments);

        /// <summary>
        /// This method invokes the method <paramref name="methodName"/> with <paramref name="arguments"/>
        /// and returning <typeparamref name="TValue"/> on <paramref name="instance"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue Invoke<TValue>(T instance, string methodName, params object[] arguments) =>
            InvokeMethod<TValue>(instance, methodName, arguments);

        /// <summary>
        /// This method invokes the method <paramref name="methodName"/> and returning <typeparamref name="TValue"/> on the existing instance
        /// </summary>
        /// <param name="methodName"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue Invoke<TValue>(string methodName) =>
            InvokeMethod<TValue>(_instance, methodName);

        /// <summary>
        /// This method invokes the method <paramref name="methodName"/> with <paramref name="arguments"/>
        /// and returning <typeparamref name="TValue"/> on the existing instance
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue Invoke<TValue>(string methodName, IEnumerable<object> arguments) =>
            InvokeMethod<TValue>(_instance, methodName, arguments);

        /// <summary>
        /// This method invokes the method <paramref name="methodName"/> with <paramref name="arguments"/>
        /// and returning <typeparamref name="TValue"/> on the existing instance
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue Invoke<TValue>(string methodName, params object[] arguments) =>
            InvokeMethod<TValue>(_instance, methodName, arguments);

        /// <summary>
        /// This method invokes the generic method <paramref name="methodName"/> with a
        /// generic of <paramref name="genericType"/> and returning <typeparamref name="TValue"/> on <paramref name="instance"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="genericType"></param>
        /// <param name="methodName"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue InvokeGeneric<TValue>(T instance, Type genericType, string methodName) =>
            InvokeMethod<TValue>(instance, methodName, null, genericType);

        /// <summary>
        /// This method invokes the generic method <paramref name="methodName"/> with a
        /// generic of <paramref name="genericType"/> and <paramref name="arguments"/>
        /// and returning <typeparamref name="TValue"/> on <paramref name="instance"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="genericType"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue InvokeGeneric<TValue>(T instance, Type genericType, string methodName,
            IEnumerable<object> arguments) =>
            InvokeMethod<TValue>(instance, methodName, arguments, genericType);

        /// <summary>
        /// This method invokes the generic method <paramref name="methodName"/> with a
        /// generic of <paramref name="genericType"/> and <paramref name="arguments"/>
        /// and returning <typeparamref name="TValue"/> on <paramref name="instance"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="genericType"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue InvokeGeneric<TValue>(T instance, Type genericType, string methodName,
            params object[] arguments) =>
            InvokeGeneric<TValue>(instance, genericType, methodName, arguments);

        /// <summary>
        /// This method invokes the generic method <paramref name="methodName"/> with a
        /// generic of <typeparamref name="TType"/> and returning the value on <paramref name="instance"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="methodName"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public TValue InvokeGeneric<TValue, TType>(T instance, string methodName) =>
             InvokeMethod<TValue>(instance, methodName, null, typeof(TType));

        /// <summary>
        /// This method invokes the generic method <paramref name="methodName"/> with a
        /// generic of <typeparamref name="TType"/> and <paramref name="arguments"/>
        /// and returning <typeparamref name="TValue"/> on <paramref name="instance"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public TValue InvokeGeneric<TValue, TType>(T instance, string methodName, IEnumerable<object> arguments) =>
            InvokeMethod<TValue>(instance, methodName, arguments, typeof(TType));

        /// <summary>
        /// This method invokes the generic method <paramref name="methodName"/> with a
        /// generic of <typeparamref name="TType"/> and <paramref name="arguments"/>
        /// and returning <typeparamref name="TValue"/> on <paramref name="instance"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public TValue InvokeGeneric<TValue, TType>(T instance, string methodName, params object[] arguments) =>
            InvokeMethod<TValue>(instance, methodName, arguments, typeof(TType));

        /// <summary>
        /// This method invokes the generic method <paramref name="methodName"/> with a
        /// generic of <paramref name="genericType"/> and <paramref name="arguments"/>
        /// and returning <typeparamref name="TValue"/> on the existing instance
        /// </summary>
        /// <param name="genericType"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue InvokeGeneric<TValue>(Type genericType, string methodName, IEnumerable<object> arguments) =>
            InvokeMethod<TValue>(_instance, methodName, arguments, genericType);

        /// <summary>
        /// This method invokes the generic method <paramref name="methodName"/> with a
        /// generic of <paramref name="genericType"/> and <paramref name="arguments"/>
        /// and returning <typeparamref name="TValue"/> on the existing instance
        /// </summary>
        /// <param name="genericType"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue InvokeGeneric<TValue>(Type genericType, string methodName, params object[] arguments) =>
            InvokeMethod<TValue>(_instance, methodName, arguments, genericType);

        /// <summary>
        /// This method invokes the generic method <paramref name="methodName"/> with a
        /// generic of <typeparamref name="TType"/> returning <typeparamref name="TValue"/>
        /// on the existing instance
        /// </summary>
        /// <param name="methodName"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public TValue InvokeGeneric<TValue, TType>(string methodName) =>
            InvokeMethod<TValue>(_instance, methodName, null, typeof(TType));

        /// <summary>
        /// This method invokes the generic method <paramref name="methodName"/> with a
        /// generic of <typeparamref name="TType"/> and <paramref name="arguments"/>
        /// and returning <typeparamref name="TValue"/> on the existing instance
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public TValue InvokeGeneric<TValue, TType>(string methodName, IEnumerable<object> arguments) =>
            InvokeMethod<TValue>(_instance, methodName, arguments, typeof(TType));

        /// <summary>
        /// This method invokes the generic method <paramref name="methodName"/> with a
        /// generic of <typeparamref name="TType"/> and <paramref name="arguments"/>
        /// and returning <typeparamref name="TValue"/> on the existing instance
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public TValue InvokeGeneric<TValue, TType>(string methodName, params object[] arguments) =>
            InvokeMethod<TValue>(_instance, methodName, arguments, typeof(TType));

        /// <summary>
        /// This method returns the properties from the instance
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, PropertyInfo> Properties() => _properties;

        /// <summary>
        /// This method returns the property information for a property
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public PropertyInfo PropertyInfo(string propertyName) =>
            _properties.FirstOrDefault(p => p.Key.Equals(propertyName.ToLower())).Value;

        /// <summary>
        /// This method returns the property information structure for a property from a lambda expression selector
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public PropertyInfo PropertyInfo(Expression<Func<T, dynamic>> selector) =>
            ExpressionToPropertyInfo(selector);

        /// <summary>
        /// This method returns the property information structure for a property from a lambda expression selector
        /// </summary>
        /// <param name="selector"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public PropertyInfo PropertyInfo<TValue>(Expression<Func<T, TValue>> selector) =>
            PropertyInfo(selector as Expression<Func<T, dynamic>>);

        /// <summary>
        /// This method returns the property name from a lambda selector expression
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public string PropertyName(Expression<Func<T, dynamic>> selector) =>
            PropertyInfo(selector)?.Name;

        /// <summary>
        /// This method sets the value of property <paramref name="propertyName"/>
        /// to <paramref name="value"/> of type <typeparamref name="TValue"/> on <paramref name="instance"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public Reflection<T> Set<TValue>(T instance, string propertyName, TValue value)
        {
            // Set the property value into the instance
            SetPropertyValue<TValue>(instance, propertyName, value);
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method sets the value of the property found with <paramref name="selector"/>
        /// to <paramref name="value"/> of type <typeparamref name="TValue"/> in <paramref name="instance"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="selector"></param>
        /// <param name="value"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public Reflection<T> Set<TValue>(T instance, Expression<Func<T, TValue>> selector, TValue value)
        {
            // Set the property value into the instance
            SetPropertyValue<TValue>(instance, selector, value);
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method sets the value of property <paramref name="propertyName"/>
        /// to <paramref name="value"/> of type <typeparamref name="TValue"/> on the existing instance
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public Reflection<T> Set<TValue>(string propertyName, TValue value) =>
            Set<TValue>(_instance, propertyName, value);

        /// <summary>
        /// This method sets the value of the property found with <paramref name="selector"/>
        /// to <paramref name="value"/> of type <typeparamref name="TValue"/> on the existing instance
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="value"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public Reflection<T> Set<TValue>(Expression<Func<T, TValue>> selector, TValue value) =>
            Set<TValue>(_instance, selector, value);

        /// <summary>
        /// This method returns the generic type of the reflection
        /// </summary>
        /// <returns></returns>
        public Type Type() => _type;

        /// <summary>
        /// This method adds a constructor argument to the instance
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public Reflection<T> WithArgument(object argument)
        {
            // Add the argument to the instance
            _arguments.Add(argument);
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method resets all of the arguments into the instance
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public Reflection<T> WithArguments(IEnumerable<object> arguments)
        {
            // Clear the arguments in the instance
            _arguments.Clear();
            // Reset the arguments into the instance
            _arguments.AddRange(arguments);
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method resets all of the arguments into the instance
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public Reflection<T> WithArguments(params object[] arguments) => WithArguments(arguments.ToList());
    }
}