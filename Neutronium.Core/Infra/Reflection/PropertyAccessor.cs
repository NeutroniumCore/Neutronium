using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Neutronium.Core.Infra.Reflection
{
    internal class PropertyAccessor
    {
        private readonly Func<object,object> _Getter;
        private readonly Action<object, object> _Setter;

        public Type TargetType { get; }
        public bool IsSettable { get; }
 
        public PropertyAccessor(Type type, PropertyInfo propertyInfo)
        {
            _Getter = BuildGet(type, propertyInfo);
            var setterInfo = (propertyInfo.CanRead) ? propertyInfo.GetSetMethod(false) : null;
            IsSettable = (setterInfo != null);
            _Setter = IsSettable ? BuildSet(type, propertyInfo) : Void;

            var originalType = propertyInfo.PropertyType;
            TargetType = originalType.GetUnderlyingType();
        }

        private static void Void(object _, object __)
        {
        }

        private static Func<object, object> BuildGet(Type type, PropertyInfo propertyInfo)
        {
            var dynamicMethod = new DynamicMethod("Get" + propertyInfo.Name, typeof(object), new[] { typeof(object)}, propertyInfo.DeclaringType, true);
            var generator = dynamicMethod.GetILGenerator();

            GenerateCreateGetPropertyIL(propertyInfo, generator);

            return (Func<object, object>)dynamicMethod.CreateDelegate(typeof(Func<object, object>));
        }

        private static void GenerateCreateGetPropertyIL(PropertyInfo propertyInfo, ILGenerator generator)
        {
            var getMethod = propertyInfo.GetGetMethod(true);
            generator.PushInstance(propertyInfo.DeclaringType);
            generator.CallMethod(getMethod);
            generator.BoxIfNeeded(propertyInfo.PropertyType);
            generator.Return();
        }

        public Action<object, object> BuildSet(Type type, PropertyInfo propertyInfo)
        {
            var dynamicMethod = new DynamicMethod("Set" + propertyInfo.Name, null, new[] { typeof(object), typeof(object) }, propertyInfo.DeclaringType, true);
            var generator = dynamicMethod.GetILGenerator();
            GenerateCreateSetPropertyIL(propertyInfo, generator);
            return (Action<object, object>)dynamicMethod.CreateDelegate(typeof(Action<object, object>));
        }

        private static void GenerateCreateSetPropertyIL(PropertyInfo propertyInfo, ILGenerator generator)
        {
            var setMethod = propertyInfo.GetSetMethod(true);
            generator.PushInstance(propertyInfo.DeclaringType);
            generator.Emit(OpCodes.Ldarg_1);
            generator.UnboxIfNeeded(propertyInfo.PropertyType);
            generator.CallMethod(setMethod);
            generator.Return();
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
