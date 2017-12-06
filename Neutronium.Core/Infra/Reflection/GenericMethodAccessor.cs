using System;
using System.Reflection;

namespace Neutronium.Core.Infra.Reflection 
{
    internal class GenericAccessor<TResult> 
    {
        private readonly MethodInfo _MethodInfo;

        internal GenericAccessor(MethodInfo methodInfo) 
        {
            _MethodInfo = methodInfo;
        }

        internal TResult Invoke(params object[] @params) 
        {
            return (TResult)_MethodInfo.Invoke(null, @params);
        }
    }

    internal class GenericMethodAccessor
    {
        private readonly MethodInfo _MethodInfo;

        private GenericMethodAccessor(IReflect type, string methodName, BindingFlags access) 
        {
            _MethodInfo = type.GetMethod(methodName, access);
        }

        public static GenericMethodAccessor Get<T>(string methodName, BindingFlags access = BindingFlags.Static | BindingFlags.NonPublic) 
        {
            return new GenericMethodAccessor(typeof(T), methodName, access);
        }

        public GenericAccessor<TResult> Build<TResult>(Type genericType) 
        {
            var resolveMethodInfo = _MethodInfo.MakeGenericMethod(genericType);
            return new GenericAccessor<TResult>(resolveMethodInfo);
        }

        public GenericAccessor<TResult> Build<TResult>(Type firstGenericType, Type secondGenericType)
        {
            var resolveMethodInfo = _MethodInfo.MakeGenericMethod(firstGenericType, secondGenericType);
            return new GenericAccessor<TResult>(resolveMethodInfo);
        }
    }
}
