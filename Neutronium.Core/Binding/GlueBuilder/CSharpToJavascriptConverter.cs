using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using MoreCollection.Extensions;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.GlueObject.Factory;
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
                [typeof(string)] = BuildString,
                [typeof(Int64)] = BuildBasic,
                [typeof(Int32)] = BuildInt,
                [typeof(Int16)] = BuildBasic,
                [typeof(UInt64)] = BuildBasic,
                [typeof(UInt32)] = BuildBasic,
                [typeof(UInt16)] = BuildBasic,
                [typeof(float)] = BuildBasic,
                [typeof(char)] = BuildBasic,
                [typeof(double)] = BuildBasic,
                [typeof(decimal)] = BuildBasic,
                [typeof(bool)] = BuildBasic,
                [typeof(DateTime)] = BuildBasic
            };
        }

        public IJsCsGlue Map(object from)
        {
            if (from == null)
                return _Null ?? (_Null = _GlueFactory.BuildBasic(null));

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

        internal bool IsBasicType(Type type) => _BasicTypes.Contains(type);

        private static IJsCsGlue BuildBasic(IGlueFactory factory, object @object) => factory.BuildBasic(@object);

        private static IJsCsGlue BuildInt(IGlueFactory factory, object @object) => factory.BuildInt((int)@object);

        private static IJsCsGlue BuildString(IGlueFactory factory, object @object) => factory.BuildString((string)@object);

        private static IJsCsGlue BuildCommand(IGlueFactory factory, object @object) => factory.Build((ICommand)@object);

        private static IJsCsGlue BuildSimpleCommand(IGlueFactory factory, object @object) => factory.Build((ISimpleCommand)@object);

        private static IJsCsGlue BuildResultCommand(IGlueFactory factory, object @object) => factory.Build((IResultCommand)@object);

        private Func<IGlueFactory, object, IJsCsGlue> GetConverter(Type type, object @object)
        {
            if (type.IsEnum)
                return BuildBasic;

            if (@object is ICommand)
                return BuildCommand;

            if (@object is ISimpleCommand)
                return BuildSimpleCommand;

            if (@object is IResultCommand)
                return BuildResultCommand;

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
