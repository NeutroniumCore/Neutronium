﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Windows.Input;
using MoreCollection.Extensions;
using Neutronium.Core.Binding.GlueBuilder;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.GlueObject.Basic;
using Neutronium.Core.Infra;
using Neutronium.Core.Infra.Reflection;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.Mapper
{
    public class CSharpToGlueMapper : ICSharpToGlueMapper
    {
        private readonly ICSharpToJsCache _Cacher;
        private readonly IGlueFactory _GlueFactory;
        private readonly IWebSessionLogger _Logger;
        private readonly Dictionary<Type, Func<IGlueFactory, object, IJsCsGlue>> _Converters;

        private IJsCsGlue _Null;

        private GlueObjectDynamicObjectBuilder _GlueObjectDynamicBuilder;
        private GlueObjectDynamicObjectBuilder GlueObjectDynamicBuilder => _GlueObjectDynamicBuilder ?? (_GlueObjectDynamicBuilder = new GlueObjectDynamicObjectBuilder(this));

        private static readonly GenericMethodAccessor _SimpleFactory = GenericMethodAccessor.Get<CSharpToGlueMapper>(nameof(BuildSimpleGenericCommand));
        private static readonly GenericMethodAccessor _CommandFactory = GenericMethodAccessor.Get<CSharpToGlueMapper>(nameof(BuildGenericCommand));
        private static readonly GenericMethodAccessor _ResultCommandFactory = GenericMethodAccessor.Get<CSharpToGlueMapper>(nameof(BuildResultCommand));
        private static readonly GenericMethodAccessor _ResultCommandWithTArgFactory = GenericMethodAccessor.Get<CSharpToGlueMapper>(nameof(BuildResultCommandWithTarg));

        public CSharpToGlueMapper(ICSharpToJsCache cacher, IGlueFactory glueFactory, IWebSessionLogger logger)
        {
            _GlueFactory = glueFactory;
            _Logger = logger;
            _Cacher = cacher;
            _Converters = new Dictionary<Type, Func<IGlueFactory, object, IJsCsGlue>>
            {
                [Types.String] = (factory, @object) => factory.BuildString(@object),
                [Types.Bool] = (factory, @object) => factory.BuildBool(@object),
                [Types.Int] = (factory, @object) => factory.BuildInt(@object),
                [Types.Double] = (factory, @object) => factory.BuildDouble(@object),
                [Types.Uint] = (factory, @object) => factory.BuildUint(@object),
                [Types.Byte] = (factory, @object) => factory.BuildByte(@object),
                [Types.SByte] = (factory, @object) => factory.BuildSByte(@object),
                [Types.Decimal] = (factory, @object) => factory.BuildDecimal(@object),
                [Types.Long] = (factory, @object) => factory.BuildLong(@object),
                [Types.Short] = (factory, @object) => factory.BuildShort(@object),
                [Types.Float] = (factory, @object) => factory.BuildFloat(@object),
                [Types.ULong] = (factory, @object) => factory.BuildUlong(@object),
                [Types.UShort] = (factory, @object) => factory.BuildUshort(@object),
                [Types.DateTime] = (factory, @object) => factory.BuildDateTime(@object),
                [Types.Char] = (factory, @object) => factory.BuildChar(@object),
            };
        }

        public IJsCsGlue Map(object from)
        {
            if (from == null)
                return _Null ?? (_Null = new JsNull());

            var res = _Cacher.GetCached(from);
            if (res != null)
                return res;

            var type = from.GetType();
            var converter = _Converters.GetOrDefault(type);
            if (converter == null)
            {
                converter = GetConverter(type, from);
                _Converters.Add(type, converter);
            }
            return converter(_GlueFactory, from);
        }

        public bool IsBasicType(Type type) => type.IsClr() || type?.IsEnum == true;

        private static IJsCsGlue BuildEnum(IGlueFactory factory, object @object) => factory.BuildEnum((Enum)@object);
        private static IJsCsGlue BuildCommand(IGlueFactory factory, object @object) => factory.Build((ICommand)@object);
        private static IJsCsGlue BuildSimpleCommand(IGlueFactory factory, object @object) => factory.Build((ISimpleCommand)@object);
        private static IJsCsGlue BuildSimpleGenericCommand<T>(IGlueFactory factory, object @object) => factory.Build((ISimpleCommand<T>)@object);
        private static IJsCsGlue BuildGenericCommand<T>(IGlueFactory factory, object @object) => factory.Build((ICommand<T>)@object);
        private static IJsCsGlue BuildCommandWithoutParameter(IGlueFactory factory, object @object) => factory.Build((ICommandWithoutParameter)@object);

        private static IJsCsGlue BuildResultCommand<TRes>(IGlueFactory factory, object @object) => factory.Build((IResultCommand<TRes>)@object);
        private static IJsCsGlue BuildResultCommandWithTarg<TArg,TRes>(IGlueFactory factory, object @object) => factory.Build((IResultCommand<TArg, TRes>)@object);

        private Func<IGlueFactory, object, IJsCsGlue> GetConverter(Type type, object @object)
        {
            if (type.IsEnum)
                return BuildEnum;

            var simpleType = type.GetInterfaceGenericType(Types.SimpleCommand);
            if (simpleType != null)
            {
                var builder = _SimpleFactory.Build<IJsCsGlue>(simpleType);
                return (fact, obj) => builder.Invoke(fact, obj);
            }

            if (@object is ISimpleCommand)
                return BuildSimpleCommand;

            simpleType = type.GetInterfaceGenericType(Types.GenericCommand);
            if (simpleType != null)
            {
                var builder = _CommandFactory.Build<IJsCsGlue>(simpleType);
                return (fact, obj) => builder.Invoke(fact, obj);
            }

            switch (@object)
            {
                case ICommandWithoutParameter _:
                    return BuildCommandWithoutParameter;

                case ICommand _:
                    return BuildCommand;
            }

            simpleType = type.GetInterfaceGenericType(Types.ResultCommand);
            if (simpleType != null)
            {
                var builder = _ResultCommandFactory.Build<IJsCsGlue>(simpleType);
                return (fact, obj) => builder.Invoke(fact, obj);
            }

            var types = type.GetInterfaceGenericTypes(Types.ResultCommandWithTArg);
            if (types != null)
            {
                var builder = _ResultCommandWithTArgFactory.Build<IJsCsGlue>(types.Item1, types.Item2);
                return (fact, obj) => builder.Invoke(fact, obj);
            }

            var stringDictionaryValueType = type.GetDictionaryStringValueType();
            if (stringDictionaryValueType != null)
            {
                var objectDictionaryBuilder = new GlueObjectDictionaryBuilder(this, stringDictionaryValueType);
                return objectDictionaryBuilder.Convert;
            }

            switch (@object)
            {
                case DynamicObject _:
                    return GlueObjectDynamicBuilder.Convert;

                case IList _:
                    return new GlueCollectionsBuilder(this, type).ConvertList;

                case ICollection _:
                    return new GlueCollectionsBuilder(this, type).ConvertCollection;

                case IEnumerable _:
                    return new GlueCollectionsBuilder(this, type).ConvertEnumerable;
            }

            var objectBuilder = new GlueObjectBuilder(this, _Logger, type);
            return objectBuilder.Convert;
        }
    }
}
