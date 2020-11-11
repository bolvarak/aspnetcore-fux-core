using System;
using System.Collections.Generic;
using System.Linq;

namespace Fux.Core
{
    /// <summary>
    /// This class provides a generically typed fluid interface to the Singleton class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class Singleton<T>
    {
        /// <summary>
        /// This method instantiates an object as a new singleton [or returns an existing one]
        /// </summary>
        /// <returns></returns>
        public static T Instance() => Singleton.Instance<T>();

        /// <summary>
        /// This method instantiates an object as a new singleton
        /// This replaces any existing singleton(s) of the same type
        /// </summary>
        /// <param name="reset"></param>
        /// <returns></returns>
        public static T Instance(bool reset) => Singleton.Instance<T>(reset);

        /// <summary>
        /// This method instantiates an object as a new singleton from a provided instance
        /// This replaces any existing singleton(s) of the same type
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static T Instance(T instance) => Singleton.Instance<T>(instance);

        /// <summary>
        /// This method instantiates an object as a new singleton that requires constructor arguments
        /// This replaces any existing singleton(s) of the same type
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static T Instance(IEnumerable<object> arguments) => Singleton.Instance<T>(arguments);

        /// <summary>
        /// This method instantiates an object as a new singleton the requires constructor arguments
        /// This replaces any existing singleton(s) of the same type
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static T Instance(params object[] arguments) => Singleton.Instance<T>(arguments);

        /// <summary>
        /// This method instantiates an object as a new singleton [or returns an existing one] and returns its reflection
        /// </summary>
        /// <returns></returns>
        public static Reflection<T> Reflect() => Singleton.Reflect<T>();

        /// <summary>
        /// This method instantiates an object as a new singleton and returns its reflection
        /// This replaces any existing singleton(s) of the same type
        /// </summary>
        /// <param name="reset"></param>
        /// <returns></returns>
        public static Reflection<T> Reflect(bool reset) => Singleton.Reflect<T>(reset);

        /// <summary>
        /// This method instantiates an object as a new singleton from a provided instance and returns its reflection
        /// This replaces any existing singleton(s) of the same type
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static Reflection<T> Reflect(T instance) => Singleton.Reflect<T>(instance);

        /// <summary>
        /// This method instantiates an object as a new singleton that requires constructor arguments and returns its reflection
        /// This replaces any existing singleton(s) of the same type
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static Reflection<T> Reflect(IEnumerable<object> arguments) => Singleton.Reflect<T>(arguments);

        /// <summary>
        /// This method instantiates an object as a new singleton that requires constructor arguments and returns its reflection
        /// This replaces any existing singleton(s) of the same type
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static Reflection<T> Reflect(params object[] arguments) => Singleton.Reflect<T>(arguments);
    }

    /// <summary>
    /// This class maintains an interface for fluidly generating Singletons
    /// </summary>
    public static class Singleton
    {
        /// <summary>
        /// This property contains the list of registered singleton instances
        /// </summary>
        private static readonly List<Reflection<dynamic>> _instances = new List<Reflection<dynamic>>();

        /// <summary>
        /// This method returns a singleton from the instance
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static dynamic instance(Type type) =>
            _instances.FirstOrDefault(i => i.Type() == type)?.Instance();

        /// <summary>
        /// This method returns a typed singleton from the instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static T instance<T>() =>
            instance(typeof(T));

        /// <summary>
        /// This method returns the reflection construct for a singleton
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static dynamic reflection(Type type) => _instances.FirstOrDefault(i => i.Type() == type);

        /// <summary>
        /// This method returns the reflection construct for a singleton
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static T reflection<T>() => reflection(typeof(T));

        /// <summary>
        /// This method fluidly adds a singleton to the instance and returns its instantiation
        /// </summary>
        /// <returns></returns>
        public static dynamic Instance(Type type)
        {
            // Check for an existing singleton and instantiate the singleton
            if (instance(type) == null) _instances.Add(Reflection.Instantiate(type) as Reflection<dynamic>);
            // We're done, return the singleton
            return instance(type);
        }

        /// <summary>
        /// This method fluidly adds or replaces a singleton in the instance with a provided instance from its system type 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="objectInstance"></param>
        /// <returns></returns>
        public static dynamic Instance(Type type, object objectInstance)
        {
            // Remove any existing singletons
            _instances.RemoveAll(i => i.Type() == type);
            // Add the provided instance
            _instances.Add(objectInstance as dynamic);
            // We're done, return the singleton
            return instance(type);
        }

        /// <summary>
        /// This method fluidly resets or adds a singleton to the instance and returns its instantiation from its system type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="reset"></param>
        /// <returns></returns>
        public static dynamic Instance(Type type, bool reset)
        {
            // Check the reset flag
            if (reset)
            {
                // Remove the existing instance
                _instances.RemoveAll(i => i.Type() == type);
                // Add the new singleton
                _instances.Add(Reflection.Instantiate(type) as Reflection<dynamic>);
            }

            // We're done, return the singleton
            return instance(type);
        }

        /// <summary>
        /// This method fluidly resets or adds a singleton to the instance with constructor arguments from its system type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static dynamic Instance(Type type, IEnumerable<object> arguments)
        {
            // Remove any existing singletons
            _instances.RemoveAll(i => i.Type() == type);
            // Add the new singleton
            _instances.Add(Reflection.Instantiate(type, arguments) as Reflection<dynamic>);
            // We're done, return the singleton
            return instance(type);
        }

        /// <summary>
        /// This method fluidly resets or adds a singleton to the instance with constructor arguments from its system type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static dynamic Instance(Type type, params object[] arguments) => Instance(type, arguments);

        /// <summary>
        /// This method fluidly adds a singleton to the instance and returns its instantiation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Instance<T>() => Instance(typeof(T));

        /// <summary>
        /// This method fluidly adds or replaces a singleton in the instance with a provided instance 
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Instance<T>(T instance) => Instance(typeof(T), instance);

        /// <summary>
        /// This method fluidly resets or adds a singleton to the instance and returns its instantiation
        /// </summary>
        /// <param name="reset"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Instance<T>(bool reset) => Instance(typeof(T), reset);

        /// <summary>
        /// This method fluidly resets or adds a singleton to the instance with constructor arguments
        /// </summary>
        /// <param name="arguments"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Instance<T>(IEnumerable<object> arguments) => Instance(typeof(T), arguments);

        /// <summary>
        /// This method fluidly resets or adds a singleton to the instance with constructor arguments
        /// </summary>
        /// <param name="arguments"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Instance<T>(params object[] arguments) => Instance(typeof(T), arguments);

        /// <summary>
        /// This method fluidly adds a singleton to the instance and returns its reflection
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Reflection<dynamic> Reflect(Type type)
        {
            // Instantiate the object
            Instance(type);
            // We're done, return the reflection
            return reflection(type);
        }

        /// <summary>
        /// This method fluidly adds or replaces a singleton in the instance with a
        /// provided instance from its system type and returns its reflection
        /// </summary>
        /// <param name="type"></param>
        /// <param name="objectInstance"></param>
        /// <returns></returns>
        public static Reflection<dynamic> Reflect(Type type, object objectInstance)
        {
            // Remove any existing singletons
            _instances.RemoveAll(i => i.Type() == type);
            // Add the provided instance
            _instances.Add(objectInstance as dynamic);
            // We're done, return the reflection
            return reflection(type);
        }

        /// <summary>
        /// This method fluidly resets or adds a singleton to the instance and
        /// returns its reflection from its system type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="reset"></param>
        /// <returns></returns>
        public static Reflection<dynamic> Reflect(Type type, bool reset)
        {
            // Instantiate the object
            Instance(type, reset);
            // We're done, return the reflection
            return reflection(type);
        }

        /// <summary>
        /// This method fluidly resets or adds a singleton to the instance with constructor arguments
        /// from its system type and returns its reflection
        /// </summary>
        /// <param name="type"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static Reflection<dynamic> Reflect(Type type, IEnumerable<object> arguments)
        {
            // Instantiate the object
            Instance(arguments);
            // We're done, return the reflection
            return reflection(type);
        }

        /// <summary>
        /// This method fluidly resets or adds a singleton to the instance with constructor arguments
        /// from its system type and returns its reflection
        /// </summary>
        /// <param name="type"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static Reflection<dynamic> Reflect(Type type, params object[] arguments) => Reflect(type, arguments);

        /// <summary>
        /// This method fluidly adds a singleton to the instance and returns its reflection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Reflection<T> Reflect<T>() => (Reflect(typeof(T)) as Reflection<T>);

        /// <summary>
        /// This method fluidly adds or replaces a singleton in the instance
        /// with a provided instance and returns its reflection 
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Reflection<T> Reflect<T>(T instance) => (Reflect(typeof(T), instance) as Reflection<T>);

        /// <summary>
        /// This method fluidly resets or adds a singleton to the instance and returns its reflection
        /// </summary>
        /// <param name="reset"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Reflection<T> Reflect<T>(bool reset) => (Reflect(typeof(T), reset) as Reflection<T>);

        /// <summary>
        /// This method fluidly resets or adds a singleton to the instance with constructor arguments and returns its reflection
        /// </summary>
        /// <param name="arguments"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Reflection<T> Reflect<T>(IEnumerable<object> arguments) =>
            (Reflect(typeof(T), arguments) as Reflection<T>);

        /// <summary>
        /// This method fluidly resets or adds a singleton to the instance with constructor arguments and returns its reflection
        /// </summary>
        /// <param name="arguments"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Reflection<T> Reflect<T>(params object[] arguments) =>
            (Reflect(typeof(T), arguments) as Reflection<T>);
    }
}