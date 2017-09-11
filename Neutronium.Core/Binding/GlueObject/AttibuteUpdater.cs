using System;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.Binding.GlueObject
{
    internal struct AttibuteUpdater
    {
        public bool IsValid => _PropertyAccessor != null;
        public bool IsSettable => IsValid && _PropertyAccessor.IsSettable;
        public object GetCurrentChildValue() => _PropertyAccessor.Get(_Father.CValue);
        public object CachedChildValue => Child.CValue;
        public string PropertyName => _PropertyAccessor.Name;
        public int Index => _PropertyAccessor.Position;
        public Type TargetType => _PropertyAccessor.TargetType;

        public IJsCsGlue Child { get; }
        private readonly PropertyAccessor _PropertyAccessor;
        private readonly JsGenericObject _Father;

        public AttibuteUpdater(JsGenericObject father, PropertyAccessor propertyAcessor, IJsCsGlue child)
        {
            Child = child;
            _PropertyAccessor = propertyAcessor;
            _Father = father;
        }

        public void Set(object value)
        {
            _PropertyAccessor.Set(_Father.CValue, value);
        }
    }
}
