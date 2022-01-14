using System;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.Binding.GlueObject
{
    public struct AttributeUpdater
    {
        public bool IsValid => PropertyAccessor != null;
        public bool IsSettable => IsValid && PropertyAccessor.IsSettable;
        public object GetCurrentChildValue() => PropertyAccessor.Get(Father.CValue);
       
        public string PropertyName => PropertyAccessor.Name;
        public Type TargetType => PropertyAccessor.TargetType;
        public IJsCsGlue Child { get; }
        public JsGenericObject Father { get; }

        public PropertyAccessor PropertyAccessor { get; }

        private object CachedChildValue => Child?.CValue;

        public AttributeUpdater(JsGenericObject father, PropertyAccessor propertyAccessor, IJsCsGlue child)
        {
            Child = child;
            PropertyAccessor = propertyAccessor;
            Father = father;
        }

        public void Set(object value)
        {
            PropertyAccessor.Set(Father.CValue, value);
        }

        public bool HasChanged(object newValue)
        {
            return !Equals(newValue, CachedChildValue);
        }
    }
}
