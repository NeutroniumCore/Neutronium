using System;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.Binding.GlueObject
{
    internal struct AttibuteUpdater
    {
        public bool IsValid => PropertyAccessor != null;
        public bool IsSettable => IsValid && PropertyAccessor.IsSettable;
        public object GetCurrentChildValue() => PropertyAccessor.Get(_Father.CValue);
       
        public string PropertyName => PropertyAccessor.Name;
        public Type TargetType => PropertyAccessor.TargetType;
        public IJsCsGlue Child { get; }

        public PropertyAccessor PropertyAccessor { get; }

        private readonly JsGenericObject _Father;
        private object CachedChildValue => Child?.CValue;

        public AttibuteUpdater(JsGenericObject father, PropertyAccessor propertyAcessor, IJsCsGlue child)
        {
            Child = child;
            PropertyAccessor = propertyAcessor;
            _Father = father;
        }

        public void Set(object value)
        {
            PropertyAccessor.Set(_Father.CValue, value);
        }

        public bool HasChanged(object newValue)
        {
            return !Equals(newValue, CachedChildValue);
        }
    }
}
