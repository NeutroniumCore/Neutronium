using System.Reflection;

namespace Neutronium.MVVMComponents.Helper 
{
    public static class TypeHelper
    {
        public static bool IsClass<T>() 
        {
#if NET472
            return typeof(T).IsClass;
#else
            return typeof(T).GetTypeInfo().IsClass;
#endif
        }
    }
}
