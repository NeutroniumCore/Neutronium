using System;
using System.Reflection;

namespace MVVM.HTML.Core.Infra
{
    internal class PropertyAccessor
    {
        private PropertyInfo _PropertyInfo;
        private readonly object _Target;
        private readonly IWebSessionLogger _Logger;

        public bool IsValid => _PropertyInfo != null;
        public bool IsSettable => IsValid && _PropertyInfo.CanWrite;
        public bool IsGettable => IsValid && _PropertyInfo.CanRead;

        public PropertyAccessor(object target, string propertyName, IWebSessionLogger logger)
        {
            _PropertyInfo = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            _Target = target;
            _Logger = logger;
        }

        public Type GetTargetType()
        {
            var originalType =_PropertyInfo.PropertyType;
            return originalType.GetUnderlyingNullableType() ?? originalType;
        }

        public bool Set(object value)
        {
            if (!IsSettable)
                return false;

            try 
            {
                _PropertyInfo.SetValue(_Target, value, null);
                return true;
            }
            catch (Exception e) 
            {
                _Logger.Info($"Unable to set C# from javascript object: property: {_PropertyInfo.Name} of {_Target}, javascript value {value}. Exception {e} was thrown.");
                return false;
            }
        }

        public object Get()
        {
            return (!IsGettable) ? null : _PropertyInfo.GetValue(_Target, null);
        }
    }
}
