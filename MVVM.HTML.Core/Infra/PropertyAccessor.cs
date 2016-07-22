﻿using System;
using System.Reflection;

namespace MVVM.HTML.Core.Infra
{
    internal class PropertyAccessor
    {
        private PropertyInfo _PropertyInfo;
        private readonly object _Target;
        public PropertyAccessor(object target, string propertyName )
        {
            _PropertyInfo = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            _Target = target;
        }

        public Type GetTargetType()
        {
            var originalType =_PropertyInfo.PropertyType;
            return originalType.GetUnderlyingNullableType() ?? originalType;
        }

        public bool IsValid => _PropertyInfo != null;

        public bool IsSettable => IsValid && _PropertyInfo.CanWrite;

        public bool IsGettable => IsValid && _PropertyInfo.CanRead;

        public bool Set(object value)
        {
            if (!IsSettable)
                return false;

            _PropertyInfo.SetValue(_Target, value, null);
            return true;
        }

        public object Get()
        {
            return (!IsGettable) ? null : _PropertyInfo.GetValue(_Target, null);
        }
    }
}
