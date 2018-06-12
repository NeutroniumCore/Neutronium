using System;
using System.Collections.Generic;
using System.Reflection;
#if NET45
using System.Dynamic;
using System.Reflection.Emit;
#endif
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using Binder = Microsoft.CSharp.RuntimeBinder.Binder;


namespace Neutronium.Core.Infra.Reflection
{
    public class PropertyAccessor
    {
        private readonly Func<object, object> _Getter;
        private readonly Action<object, object> _Setter;

        public Type TargetType { get; }
        public bool IsSettable { get; }
        public int Position { get; set; }
        public string Name { get; }

        public PropertyAccessor(Type type, PropertyInfoDescription propertyInfoDescription, int position)
        {
            Position = position;
            var propertyInfo = propertyInfoDescription.PropertyInfo;
            Name = propertyInfo.Name;
            _Getter = BuildGet(propertyInfo);
            var setterInfo = propertyInfoDescription.IsSettable ? propertyInfo.GetSetMethod(false) : null;
            IsSettable = (setterInfo != null);
            _Setter = IsSettable ? BuildSet(type, propertyInfo) : Void;

            var originalType = propertyInfo.PropertyType;
            TargetType = originalType.GetUnderlyingType();
        }

        private PropertyAccessor(Type type, string name, int position, Accessor accessor)
        {
            Position = position;
            Name = name;
            TargetType = type;
            IsSettable = true;
            _Getter = accessor.Getter;
            _Setter = accessor.Setter;
        }

        public static PropertyAccessor FromDictionary<T>(string name, int position)
        {
            var accessor = GetDictionaryAcessor<T>(name);
            return new PropertyAccessor(typeof(T), name, position, accessor);
        }

        public static PropertyAccessor FromDynamicObject(Type type, string name, int position)
        {
            var accessor = GetDynamicObjectAcessor(type, name);
            return new PropertyAccessor(Types.Object, name, position, accessor);
        }

        private struct Accessor
        {
            public Func<object, object> Getter { get; set; }
            public Action<object, object> Setter { get; set; }
        }

        private static Accessor GetDynamicObjectAcessor(Type type, string attributeName)
        {
            var getterBinder = Binder.GetMember(CSharpBinderFlags.None, attributeName, type, new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
            var callsiteGetter = CallSite<Func<CallSite, object, object>>.Create(getterBinder);

            var setterBinder = Binder.SetMember(CSharpBinderFlags.None, attributeName, type, new[]
            {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
            });
            var callsiteSetter = CallSite<Func<CallSite, object, object, object>>.Create(setterBinder);

            return new Accessor {
                Setter = (@object, value) => callsiteSetter.Target(callsiteSetter, @object, value),
                Getter = (@object) => callsiteGetter.Target(callsiteGetter, @object)
            };
        }

        private static Accessor GetDictionaryAcessor<TValue>(string attributeName)
        {
            return new Accessor
            {
                Setter = (@object, value) => ((IDictionary<string, TValue>)@object)[attributeName] = (TValue)value,
                Getter = (@object) => ((IDictionary<string, TValue>)@object).GetOrNull(attributeName)
            };
        }

        private static void Void(object _, object __) {}

        private static Func<object, object> BuildGet(PropertyInfo propertyInfo)
        {
#if NET45
            var dynamicMethod = new DynamicMethod("Get" + propertyInfo.Name, Types.Object, new[] { Types.Object }, propertyInfo.DeclaringType, true);
            var generator = dynamicMethod.GetILGenerator();
            GenerateCreateGetPropertyIL(propertyInfo, generator);
            return (Func<object, object>)dynamicMethod.CreateDelegate(typeof(Func<object, object>));
#else
            Func<object, object> getter = (@object) => propertyInfo.GetValue(@object);
            return getter;
#endif
        }

#if NET45
        private static void GenerateCreateGetPropertyIL(PropertyInfo propertyInfo, ILGenerator generator)
        {
            var getMethod = propertyInfo.GetGetMethod(true);
            generator.PushInstance(propertyInfo.DeclaringType);
            generator.CallMethod(getMethod);
            generator.BoxIfNeeded(propertyInfo.PropertyType);
            generator.Return();
        }
#endif

        public Action<object, object> BuildSet(Type type, PropertyInfo propertyInfo)
        {
#if NET45
            var dynamicMethod = new DynamicMethod("Set" + propertyInfo.Name, null, new[] { Types.Object, Types.Object }, propertyInfo.DeclaringType, true);
            var generator = dynamicMethod.GetILGenerator();
            GenerateCreateSetPropertyIL(propertyInfo, generator);
            return (Action<object, object>)dynamicMethod.CreateDelegate(typeof(Action<object, object>));
#else
            Action<object, object> setter = (@object, value) => propertyInfo.SetValue(@object, value);
            return setter;
#endif
        }

#if NET45
        private static void GenerateCreateSetPropertyIL(PropertyInfo propertyInfo, ILGenerator generator)
        {
            var setMethod = propertyInfo.GetSetMethod(true);
            generator.PushInstance(propertyInfo.DeclaringType);
            generator.Emit(OpCodes.Ldarg_1);
            generator.UnboxIfNeeded(propertyInfo.PropertyType);
            generator.CallMethod(setMethod);
            generator.Return();
        }
#endif

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
