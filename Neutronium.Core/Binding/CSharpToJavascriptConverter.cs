using System;
using System.Collections;
using System.Windows.Input;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.MVVMComponents;
using System.Collections.Generic;
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

        public IJsCsGlue Map(object from, object additional=null)
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

            var ienfro = from as IEnumerable;
            if (ienfro!=null)
                return  Convert(ienfro);

            var propertyInfos = @from.GetType().GetReadProperties();
            var additionalPropertyInfos = additional?.GetType().GetReadProperties();

            var gres = _GlueFactory.Build(from, propertyInfos.Count + (additionalPropertyInfos?.Count ?? 0));

            MapNested(gres, @from, propertyInfos);
            MapNested(gres, additional, additionalPropertyInfos);
            return gres;
        }

        private void MapNested(JsGenericObject gres, object parentObject, ICollection<PropertyAccessor> properties)
        {
            if (parentObject == null)
                return;

            foreach (var propertyInfo in properties)
            {
                var propertyName = propertyInfo.Name;
                object childvalue;
                try
                {
                    childvalue = propertyInfo.Get(parentObject); 
                }
                catch(Exception e)
                {
                    _Logger.Info(()=> $"Unable to convert property {propertyName} from {parentObject} of type {parentObject.GetType().FullName} exception {e.InnerException}");
                    continue;
                }

                var childres = Map(childvalue);          
                gres.AddGlueProperty(propertyName, childres);
            }
        }

        private IJsCsGlue Convert(IEnumerable source)
        {
            var type = source.GetElementType();
            var basictype = _Context.IsTypeBasic(type) ? type : null;
            var count = (source as ICollection)?.Count;
            var list = count.HasValue? new List<IJsCsGlue>(count.Value): new List<IJsCsGlue>();
            foreach (var @object in source)
            {
                list.Add(Map(@object));
            }

            return _GlueFactory.BuildArray(list, source, basictype);
        }
    }
}
