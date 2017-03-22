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

        public static ITypesProvider Implementing<T>(this ITypesProvider typeProvider)
        {
            return new TypesProvider(typeProvider.Types.Where(t => t.IsAssignableFrom(typeof(T))));
        }

        public static ITypesProvider WithNameContaining(this ITypesProvider typeProvider, string value)
        {
            return new TypesProvider(typeProvider.Types.Where(t => t.Name.Contains(value)));
        }

        public static ITypesProvider InNamespace(this ITypesProvider typeProvider, string value)
        {
            return new TypesProvider(typeProvider.Types.Where(t => t.Namespace == value));
        }

        public static ITypesProvider And(this ITypesProvider typeProvider, ITypesProvider typeProvider2)
        {
            return new TypesProvider(typeProvider.Types.Concat(typeProvider2.Types));
        }

        public static IConventionRouter Register(this IConventionRouter conventionRouter, ITypesProvider typeProvider)
        {
            return conventionRouter.Register(typeProvider.Types);
        }
    }
}
