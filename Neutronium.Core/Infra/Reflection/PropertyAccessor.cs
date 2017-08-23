using System;
using System.Reflection;

namespace Neutronium.Core.Infra.Reflection
{
    internal class PropertyAccessor
    {
        private readonly PropertyInfo _PropertyInfo;
        private readonly Func<object,object> _Getter;
        private readonly Action<object, object> _Setter;
        public string Name => _PropertyInfo.Name;

        public bool IsSettable { get; }
 
        private static readonly MethodInfo _BuildGet = typeof(PropertyAccessor).GetMethod(nameof(BuildGetGeneric), BindingFlags.Static | BindingFlags.NonPublic);
        private static readonly MethodInfo _BuildSet = typeof(PropertyAccessor).GetMethod(nameof(BuildSetGeneric), BindingFlags.Static | BindingFlags.NonPublic);

        public PropertyAccessor(Type type, PropertyInfo propertyInfo)
        {
            _PropertyInfo = propertyInfo;
            _Getter = BuildGet(type, propertyInfo);
            var setterInfo = (propertyInfo.CanRead) ? propertyInfo.GetSetMethod(false) : null;
            IsSettable = (setterInfo != null);
            _Setter = IsSettable ? BuildSet(type, propertyInfo.PropertyType, setterInfo) : Void;
        }

        public Type GetTargetType()
        {
            var originalType = _PropertyInfo.PropertyType;
            return originalType.GetUnderlyingType();
        }

        private static void Void(object _, object __)
        {
        }

        private static Func<object, object> BuildGet(Type type, PropertyInfo propertyInfo)
        {
            var buildMethod = _BuildGet.MakeGenericMethod(type, propertyInfo.PropertyType);
            return (Func<object, object>)buildMethod.Invoke(null, new[] { propertyInfo });
        }

        private static Func<object, object> BuildGetGeneric<TObject,TValue>(PropertyInfo propertyInfo)
        {
            var getDelegate = (Func<TObject, TValue>)Delegate.CreateDelegate(typeof(Func<TObject, TValue>), propertyInfo.GetGetMethod(nonPublic: false));
            return (@object) => getDelegate((TObject)@object);
        }

        private static Action<object, object> BuildSet(Type type, Type targetType, MethodInfo propertysetInfo)
        {
            var buildMethod = _BuildSet.MakeGenericMethod(type, targetType);
            return (Action<object, object>)buildMethod.Invoke(null, new[] { propertysetInfo });
        }

        private static Action<object, object> BuildSetGeneric<TObject, TValue>(MethodInfo propertysetInfo)
        {
            var getDelegate = (Action<TObject, TValue>)Delegate.CreateDelegate(typeof(Action<TObject, TValue>), propertysetInfo);
            return (@object, value) => getDelegate((TObject)@object, (TValue)value);
        }

        public void Set(object target, object value)
        {
            _Setter(target, value);
        }

        public object Get(object target)
        {
            return _Getter(target);
        }
    }
}
