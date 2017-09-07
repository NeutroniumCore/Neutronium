using System;
using System.Collections;
using System.Windows.Input;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.MVVMComponents;
using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject.Factory;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.Core.Binding.GlueBuilder;
using MoreCollection.Extensions;

namespace Neutronium.Core.Binding
{
    internal class CSharpToJavascriptConverter 
    {
        private readonly ICSharpToJsCache _Cacher;
        private readonly IGlueFactory _GlueFactory;
        private readonly IWebSessionLogger _Logger;
        private readonly IWebBrowserWindow _Context;
        private IJsCsGlue _Null;
        private readonly Dictionary<Type, ICsToGlueConverter> _Converters;
        private readonly GlueBasicBuilder _BasicConverter;
        private readonly GlueCommandsBuilder _CommandsBuilder;
        private readonly GlueCollectionsBuilder _GlueCollectionsBuilder;

        public CSharpToJavascriptConverter(IWebBrowserWindow context, ICSharpToJsCache cacher, IGlueFactory glueFactory, IWebSessionLogger logger) {
            _Context = context;
            _GlueFactory = glueFactory;
            _Logger = logger;
            _Cacher = cacher;
            _BasicConverter = new GlueBasicBuilder(_GlueFactory);
            _CommandsBuilder = new GlueCommandsBuilder(_GlueFactory);
            _GlueCollectionsBuilder = new GlueCollectionsBuilder(_GlueFactory, _Context, this);
            _Converters = new Dictionary<Type, ICsToGlueConverter> 
            {
                [typeof(string)] = _BasicConverter,
                [typeof(Int64)] = _BasicConverter,
                [typeof(Int32)] = _BasicConverter,
                [typeof(Int16)] = _BasicConverter,
                [typeof(UInt64)] = _BasicConverter,
                [typeof(UInt32)] = _BasicConverter,
                [typeof(UInt16)] = _BasicConverter,
                [typeof(float)] = _BasicConverter,
                [typeof(char)] = _BasicConverter,
                [typeof(double)] = _BasicConverter,
                [typeof(decimal)] = _BasicConverter,
                [typeof(bool)] = _BasicConverter,
                [typeof(DateTime)] = _BasicConverter
            };
        }

        public IJsCsGlue Map(object from, object additional) 
        {
            var result = Map(from);
            var root = result as JsGenericObject;
            if ((root == null) || (additional == null))
                return result;

            var other = Map(additional) as JsGenericObject;
            root.Merge(other);
            return root;
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

            return converter.Convert(from);
        }

        private ICsToGlueConverter GetConverter(Type type, object @object) 
        {
            if (type.IsEnum)
                return _BasicConverter;

            if (@object is ICommand)
                return _CommandsBuilder.Command;

            if (@object is ISimpleCommand)
                return _CommandsBuilder.SimpleCommand;

            if (@object is IResultCommand)
                return _CommandsBuilder.ResultCommand;

            if (@object is IList)
                return _GlueCollectionsBuilder.GetList(type);

            if (@object is ICollection)
                return _GlueCollectionsBuilder.GetCollection(type);

            if (@object is IEnumerable)
                return _GlueCollectionsBuilder.GetEnumerable(type);

            return new GlueObjectBuilder(_GlueFactory, this, _Logger, type);
        }
    }
}
