using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Windows.Input;
using MoreCollection.Extensions;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.GlueObject.Basic;
using Neutronium.Core.Infra;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueBuilder
{
    internal class CSharpToJavascriptConverter
    {
        private readonly ICSharpToJsCache _Cacher;
        private readonly IGlueFactory _GlueFactory;
        private readonly IWebSessionLogger _Logger;
        private IJsCsGlue _Null;
        private readonly Dictionary<Type, Func<IGlueFactory, object, IJsCsGlue>> _Converters;

        private GlueObjectDynamicObjectBuilder _GlueObjectDynamicBuilder;
        private GlueObjectDynamicObjectBuilder GlueObjectDynamicBuilder => 
            _GlueObjectDynamicBuilder ?? (_GlueObjectDynamicBuilder =new GlueObjectDynamicObjectBuilder(this));

        private static readonly HashSet<Type> _BasicTypes = new HashSet<Type>
        {
            typeof(string),
            typeof(Int64),
            typeof(Int32),
            typeof(Int16),
            typeof(UInt64),
            typeof(UInt32),
            typeof(UInt16),
            typeof(float),
            typeof(char),
            typeof(double),
            typeof(decimal),
            typeof(bool),
            typeof(DateTime)
        };

        public CSharpToJavascriptConverter(ICSharpToJsCache cacher, IGlueFactory glueFactory, IWebSessionLogger logger)
        {
            _GlueFactory = glueFactory;
            _Logger = logger;
            _Cacher = cacher;
            _Converters = new Dictionary<Type, Func<IGlueFactory, object, IJsCsGlue>>
            {
                [typeof(string)] = (factory, @object) => factory.BuildString(@object),
                [typeof(bool)]   = (factory, @object) => factory.BuildBool(@object),
                [typeof(int)]    = (factory, @object) => factory.BuildInt(@object),
                [typeof(double)] = (factory, @object) => factory.BuildDouble(@object),
                [typeof(uint)]   = (factory, @object) => factory.BuildUint(@object),
                [typeof(decimal)] = (factory, @object) => factory.BuildDecimal(@object),
                [typeof(long)] =   (factory, @object) => factory.BuildLong(@object),
                [typeof(short)] =  (factory, @object) => factory.BuildShort(@object),
                [typeof(float)] =  (factory, @object) => factory.BuildFloat(@object),
                [typeof(ulong)] =  (factory, @object) => factory.BuildUlong(@object),
                [typeof(ushort)] = (factory, @object) => factory.BuildUshort(@object),
                [typeof(DateTime)] = (factory, @object) => factory.BuildDateTime(@object),
                [typeof(char)] = (factory, @object) => factory.BuildChar(@object),                          
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

        internal bool IsBasicType(Type type) => _BasicTypes.Contains(type) || type?.IsEnum == true;

        private static IJsCsGlue BuildEnum(IGlueFactory factory, object @object) => factory.BuildEnum((Enum)@object);
        private static IJsCsGlue BuildCommand(IGlueFactory factory, object @object) => factory.Build((ICommand)@object);
        private static IJsCsGlue BuildSimpleCommand(IGlueFactory factory, object @object) => factory.Build((ISimpleCommand)@object);
        private static IJsCsGlue BuildResultCommand(IGlueFactory factory, object @object) => factory.Build((IResultCommand)@object);

        private Func<IGlueFactory, object, IJsCsGlue> GetConverter(Type type, object @object)
        {
            if (type.IsEnum)
                return BuildEnum;

            if (@object is ICommand)
                return BuildCommand;

            if (@object is ISimpleCommand)
                return BuildSimpleCommand;

            if (@object is IResultCommand)
                return BuildResultCommand;

            var stringDictioanryValueType = type.GetDictionaryStringValueType();
            if (stringDictioanryValueType!= null)
            {
                var objectDictionaryBuilder = new GlueObjectDictionaryBuilder(this, stringDictioanryValueType);
                return objectDictionaryBuilder.Convert;
            }

            var dynamicObject = @object as DynamicObject;
            if (dynamicObject != null)
                return GlueObjectDynamicBuilder.Convert;

            if (@object is IList)
                return new GlueCollectionsBuilder(this, type).ConvertList;

            if (@object is ICollection)
                return new GlueCollectionsBuilder(this, type).ConvertCollection;

            if (@object is IEnumerable)
                return new GlueCollectionsBuilder(this, type).ConvertEnumerable;

            var objectBuilder = new GlueObjectBuilder(this, _Logger, type);
            return objectBuilder.Convert;
        }
    }
}
