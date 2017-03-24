using System;
using System.Linq;
using System.Reflection;

namespace Neutronium.Core.Navigation.Routing
{
    public static class TypesProviderExtension
    {
        /// <summary>
        /// creates a ITypesProvider with all types of the corresponding assembly
        /// <seealso cref="ITypesProvider"/>
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns>
        /// a ITypesProvider with all types of the corresponding assembly
        /// </returns>
        public static ITypesProvider GetAllTypes(this Assembly assembly)
        {
            return new TypesProvider(assembly.GetTypes());
        }

        /// <summary>
        /// creates a ITypesProvider with types of the type assembly
        /// <seealso cref="ITypesProvider"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns>
        /// a ITypesProvider with all types of the type assembly
        /// </returns>
        public static ITypesProvider GetTypesFromSameAssembly(this Type type)
        {
            return GetAllTypes(type.Assembly);
        }

        /// <summary>
        /// creates a ITypesProvider with types of the object assembly
        /// <seealso cref="ITypesProvider"/>
        /// </summary>
        /// <param name="object"></param>
        /// <returns>
        /// a ITypesProvider with all types of the type assembly
        /// </returns>
        public static ITypesProvider GetTypesFromSameAssembly(this object @object)
        {
            return GetTypesFromSameAssembly(@object.GetType());
        }

        /// <summary>
        /// Filter ITypesProvider returning only types implementing a given base type 
        /// <seealso cref="ITypesProvider"/>
        /// </summary>
        /// <typeparam name="T">
        /// type to filter
        /// </typeparam>
        /// <param name="typeProvider"></param>
        /// <returns>
        /// a new ITypesProvider with types deriving from given type
        /// </returns>
        public static ITypesProvider Implementing<T>(this ITypesProvider typeProvider)
        {
            var type = typeof(T);
            return new TypesProvider(typeProvider.Types.Where(t => type != t && type.IsAssignableFrom(t)));
        }

        /// <summary>
        /// Filter ITypesProvider returning only types with name ending by given value
        /// <seealso cref="ITypesProvider"/>
        /// </summary>
        /// <param name="typeProvider"></param>
        /// <param name="value">
        /// string filter
        /// </param>
        /// <returns>
        /// a new ITypesProvider with types which names ends by given value
        /// </returns>
        public static ITypesProvider WithNameEndingWith(this ITypesProvider typeProvider, string value)
        {
            return new TypesProvider(typeProvider.Types.Where(t => t.Name.EndsWith(value)));
        }

        /// <summary>
        /// Filter ITypesProvider returning only types in a given namespace
        /// <seealso cref="ITypesProvider"/>
        /// </summary>
        /// <param name="typeProvider"></param>
        /// <param name="value">
        /// namespace name
        /// </param>
        /// <returns>
        /// a new ITypesProvider with types in the given namespace
        /// </returns>
        public static ITypesProvider InNamespace(this ITypesProvider typeProvider, string value)
        {
            return new TypesProvider(typeProvider.Types.Where(t => t.Namespace == value));
        }

        /// <summary>
        /// Add types of two ITypesProvider
        /// <seealso cref="ITypesProvider"/>
        /// </summary>
        /// <param name="typesProvider"></param>
        /// <param name="typesProvider2"></param>
        /// <returns>
        /// a new ITypesProvider with types of both typeProvider
        /// </returns>
        public static ITypesProvider Add(this ITypesProvider typesProvider, ITypesProvider typesProvider2)
        {
            return new TypesProvider(typesProvider.Types.Concat(typesProvider2.Types));
        }

        /// <summary>
        /// Add assembly types to a given ITypesProvider
        /// <seealso cref="ITypesProvider"/>
        /// </summary>
        /// <param name="typeProvider"></param>
        /// <param name="assembly"></param>
        /// <returns>
        /// a new ITypesProvider with original types and types from the given assembly
        /// </returns>
        public static ITypesProvider AddTypesFrom(this ITypesProvider typeProvider, Assembly assembly)
        {
            return typeProvider.Add(assembly.GetAllTypes());
        }

        /// <summary>
        /// Add types of the assembly of a given type to a given ITypesProvider
        /// <seealso cref="ITypesProvider"/>
        /// </summary>
        /// <param name="typeProvider"></param>
        /// <param name="type"></param>
        /// <returns>
        /// a new ITypesProvider with original types and types of the assembly of the given type
        /// </returns>
        public static ITypesProvider AddTypesOfSameAssembly(this ITypesProvider typeProvider, Type type)
        {
            return typeProvider.Add(type.GetTypesFromSameAssembly());
        }

        /// <summary>
        /// Add types of the assembly of a given object to a given ITypesProvider
        /// <seealso cref="ITypesProvider"/>
        /// </summary>
        /// <param name="typeProvider"></param>
        /// <param name="object"></param>
        /// <returns>
        /// a new ITypesProvider with original types and types of the assembly of the given object
        /// </returns>
        public static ITypesProvider AddTypesOfSameAssembly(this ITypesProvider typeProvider, object @object)
        {
            return typeProvider.Add(@object.GetTypesFromSameAssembly());
        }

        /// <summary>
        /// Add types of the two ITypesProvider
        /// <seealso cref="ITypesProvider"/>
        /// </summary>
        /// <param name="typeProvider"></param>
        /// <param name="types"></param>
        /// <returns>
        /// a new ITypesProvider containing both types
        /// </returns>
        public static ITypesProvider Add(this ITypesProvider typeProvider, params Type[] types)
        {
            return new TypesProvider(typeProvider.Types.Concat(types));
        }

        /// <summary>
        /// Filter types based on a given condition
        /// <seealso cref="ITypesProvider"/>
        /// </summary>
        /// <param name="typeProvider"></param>
        /// <param name="filter">
        /// type filter
        /// </param>
        /// <returns>
        /// a new ITypesProvider containing only type of given ITypesProvider corresponding
        /// to the given condition
        /// </returns>
        public static ITypesProvider Where(this ITypesProvider typeProvider, Func<Type, bool> filter)
        {
            return new TypesProvider(typeProvider.Types.Where(filter));
        }

        /// <summary>
        /// Filter ITypesProvider types based on a given condition
        /// <seealso cref="ITypesProvider"/>
        /// </summary>
        /// <param name="typeProvider"></param>
        /// <param name="filter">
        /// type filter: if type matches the criteria it will be removed from result
        /// </param>
        /// <returns>
        /// a new ITypesProvider containing only type of given ITypesProvider corresponding
        /// that do not match the filter
        /// </returns>
        public static ITypesProvider Except(this ITypesProvider typeProvider, Func<Type, bool> filter)
        {
            return new TypesProvider(typeProvider.Types.Where(t => !filter(t)));
        }

        /// <summary>
        /// Create a new ITypesProvider without the given types
        /// </summary>
        /// <param name="typeProvider"></param>
        /// <param name="types">
        /// types to be removed from result
        /// </param>
        /// <returns>
        /// a new ITypesProvider not containing the given types
        /// </returns>
        public static ITypesProvider Except(this ITypesProvider typeProvider, params Type[] types)
        {
            return new TypesProvider(typeProvider.Types.Except(types));
        }

        /// <summary>
        /// Register all the types provided by typeProvider
        /// using the current convention
        /// </summary>
        /// <param name="conventionRouter">
        /// Convention router
        /// </param>
        /// <param name="typeProvider">
        /// type provider 
        /// </param>
        /// <returns>
        /// the corresponding convention router
        /// </returns>
        public static IConventionRouter Register(this IConventionRouter conventionRouter, ITypesProvider typeProvider)
        {
            return conventionRouter.Register(typeProvider.Types.Distinct());
        }
    }
}
