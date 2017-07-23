using System;
using System.Reflection;

namespace Neutronium.Core.Infra
{
    internal class PropertyAccessor
    {
        private readonly PropertyInfo _PropertyInfo;
        private readonly object _Target;
        private readonly IWebSessionLogger _Logger;

        public bool IsValid => _PropertyInfo != null;
        private bool? _IsSettable;
        private bool? _IsGettable;
        public bool IsSettable => _IsSettable?? (_IsSettable = IsValid && _PropertyInfo.CanWrite && _PropertyInfo.GetSetMethod(false) != null).Value;
        public bool IsGettable => _IsGettable?? (_IsGettable = IsValid && _PropertyInfo.CanRead && _PropertyInfo.GetGetMethod(false) != null).Value;

        public PropertyAccessor(object target, string propertyName, IWebSessionLogger logger)
        {
            _PropertyInfo = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            _Target = target;
            _Logger = logger;
        }

        public Type GetTargetType()
        {
            var originalType =_PropertyInfo.PropertyType;
            return originalType.GetUnderlyingType();
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
