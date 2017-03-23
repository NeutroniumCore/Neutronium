using System;
using System.Linq;
using System.Reflection;

namespace Neutronium.Core.Navigation.Routing
{
    public static class TypesProviderExtension
    {
        public static ITypesProvider GetAllTypes(this Assembly assembly)
        {
            return new TypesProvider(assembly.GetTypes());
        }

        public static ITypesProvider GetTypesFromSameAssembly(this Type type)
        {
            return GetAllTypes(type.Assembly);
        }

        public static ITypesProvider GetTypesFromSameAssembly(this object @object)
        {
            return GetTypesFromSameAssembly(@object.GetType());
        }

        public static ITypesProvider Implementing<T>(this ITypesProvider typeProvider)
        {
            var type = typeof(T);
            return new TypesProvider(typeProvider.Types.Where(t => type != t && type.IsAssignableFrom(t)));
        }

        public static ITypesProvider WithNameEndingWith(this ITypesProvider typeProvider, string value)
        {
            return new TypesProvider(typeProvider.Types.Where(t => t.Name.EndsWith(value)));
        }

        public static ITypesProvider InNamespace(this ITypesProvider typeProvider, string value)
        {
            return new TypesProvider(typeProvider.Types.Where(t => t.Namespace == value));
        }

        public static ITypesProvider Add(this ITypesProvider typeProvider, ITypesProvider typeProvider2)
        {
            return new TypesProvider(typeProvider.Types.Concat(typeProvider2.Types));
        }

        public static ITypesProvider AddTypesFrom(this ITypesProvider typeProvider, Assembly assembly)
        {
            return typeProvider.Add(assembly.GetAllTypes());
        }

        public static ITypesProvider AddTypesOfSameAssembly(this ITypesProvider typeProvider, Type type)
        {
            return typeProvider.Add(type.GetTypesFromSameAssembly());
        }

        public static ITypesProvider AddTypesOfSameAssembly(this ITypesProvider typeProvider, object @object)
        {
            return typeProvider.Add(@object.GetTypesFromSameAssembly());
        }

        public static ITypesProvider Add(this ITypesProvider typeProvider, params Type[] types)
        {
            return new TypesProvider(typeProvider.Types.Concat(types));
        }

        public static ITypesProvider Where(this ITypesProvider typeProvider, Func<Type, bool> filter)
        {
            return new TypesProvider(typeProvider.Types.Where(filter));
        }

        public static ITypesProvider Except(this ITypesProvider typeProvider, Func<Type, bool> filter)
        {
            return new TypesProvider(typeProvider.Types.Where(t => !filter(t)));
        }

        public static ITypesProvider Except(this ITypesProvider typeProvider, params Type[] types)
        {
            return new TypesProvider(typeProvider.Types.Except(types));
        }

        public static IConventionRouter Register(this IConventionRouter conventionRouter, ITypesProvider typeProvider)
        {
            return conventionRouter.Register(typeProvider.Types);
        }
    }
}
