using System;
using System.Collections;
using System.Windows.Input;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.MVVMComponents;
using System.Collections.Generic;
using System.Threading;
using Neutronium.Core.Binding.GlueObject.Factory;
using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.Binding
{
    internal class CSharpToJavascriptConverter
    {
        private readonly ICSharpToJsCache _Cacher;
        private readonly IGlueFactory _GlueFactory;
        private readonly IWebSessionLogger _Logger;
        private readonly IWebBrowserWindow _Context;
        private IJsCsGlue _Null;

        public CSharpToJavascriptConverter(IWebBrowserWindow context, ICSharpToJsCache cacher, IGlueFactory glueFactory, IWebSessionLogger logger)
        {
            _Context = context;
            _GlueFactory = glueFactory;
            _Logger = logger;
            _Cacher = cacher;
        }

        public IJsCsGlue Map(object from, object additional)
        {
            var result = Map(from);
            var root = result as JsGenericObject;
            if ((root == null) || (additional == null))
                return result;

            var properties = MapNested(additional);
            root.SetAttributes(properties);
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
            if (_Context.IsTypeBasic(type))
                return _GlueFactory.BuildBasic(from);

            var command = from as ICommand;
            if (command != null)
                return _GlueFactory.Build(command);

            var simpleCommand = from as ISimpleCommand;
            if (simpleCommand != null)
                return _GlueFactory.Build(simpleCommand);

            var resultCommand = from as IResultCommand;
            if (resultCommand != null)
                return _GlueFactory.Build(resultCommand);

            if (type.IsEnum)
                return _GlueFactory.BuildBasic(from);

            var enumerable = from as IEnumerable;
            if (enumerable != null)
            {
                var arrayResult = CreateArray(enumerable);
                arrayResult.SetChildren(Convert(enumerable));
                return arrayResult;
            }     

            var result = _GlueFactory.Build(from);
            result.SetAttributes(MapNested(@from));
            return result;
        }

        private AttributeDescription[] MapNested(object parentObject)
        {
            if (parentObject == null)
                return null;

            var properties = parentObject.GetType().GetReadProperties();
            var attributes = new AttributeDescription[properties.Count];
            var index = 0;

            foreach (var propertyInfo in properties)
            {
                var propertyName = propertyInfo.Key;
                object childvalue = null;
                try
                {
                    childvalue = propertyInfo.Value.Get(parentObject);
                }
                catch (Exception e)
                {
                    _Logger.Info(() => $"Unable to convert property {propertyName} from {parentObject} of type {parentObject.GetType().FullName} exception {e.InnerException}");
                }

                var child = Map(childvalue).AddRef();
                attributes[index++] = new AttributeDescription(propertyName, child);
            }
            return attributes;
        }

        private JsArray CreateArray(IEnumerable enumerable)
        {
            var elementType = enumerable.GetElementType();
            var basictype = _Context.IsTypeBasic(elementType) ? elementType : null;

            return _GlueFactory.BuildArray(enumerable, basictype);
        }

        private List<IJsCsGlue> Convert(IEnumerable source)
        {
            var collection = (source as ICollection);
            var list = (collection != null) ? new List<IJsCsGlue>(collection.Count) : new List<IJsCsGlue>();
            foreach (var @object in source)
            {
                list.Add(Map(@object).AddRef());
            }
            return list;
        }
    }
}
