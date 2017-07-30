using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.MVVMComponents;
using System.Collections.Generic;
using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.Core.Binding
{
    internal class CSharpToJavascriptConverter 
    {
        private readonly IJavascriptSessionCache _Cacher;
        private readonly IGlueFactory _GlueFactory;
        private readonly IWebSessionLogger _Logger;
        private readonly IWebBrowserWindow _Context;
        private IJSCSGlue _Null;

        public CSharpToJavascriptConverter(IWebBrowserWindow context, IJavascriptSessionCache cacher, IGlueFactory glueFactory, IWebSessionLogger logger)
        {
            _Context = context;
            _GlueFactory = glueFactory;
            _Logger = logger;
            _Cacher = cacher;
        }

        public IJSCSGlue Map(object from, object additional=null)
        {
            if (from == null)
                return _Null ?? (_Null = new JSBasicObject(null));

            var res = _Cacher.GetCached(from);
            if (res != null)
                return res;

            var type = from.GetType();
            if (_Context.IsTypeBasic(type))
            {
                res = new JSBasicObject(from);
                _Cacher.Cache(from, res);
                return res;
            }             

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
            {
                var trueres = new JSBasicObject(from);
                _Cacher.Cache(from, trueres);
                return trueres;
            }

            var ienfro = from as IEnumerable;
            if (ienfro!=null)
                return  Convert(ienfro);

            var propertyInfos = @from.GetType().GetReadProperties();
            var additionalPropertyInfos = additional?.GetType().GetReadProperties();

            var gres = _GlueFactory.Build(from, propertyInfos.Count + (additionalPropertyInfos?.Count ?? 0));

            _Cacher.Cache(from, gres);

            MappNested(gres, @from, propertyInfos);
            MappNested(gres, additional, additionalPropertyInfos);
            return gres;
        }

        private void MappNested(JsGenericObject gres, object parentObject, IList<PropertyInfo> properties)
        {
            if (parentObject == null)
                return;

            foreach (var propertyInfo in properties)
            {
                var propertyName = propertyInfo.Name;
                object childvalue;
                try
                {
                    childvalue = propertyInfo.GetValue(parentObject, null); 
                }
                catch(TargetInvocationException e)
                {
                    _Logger.Info(()=> $"Unable to convert property {propertyName} from {parentObject} of type {parentObject.GetType().FullName} exception {e.InnerException}");
                    continue;
                }

                var childres = Map(childvalue);          
                gres.AddGlueProperty(propertyName, childres);
            }
        }

        private IJSCSGlue Convert(IEnumerable source)
        {
            var type = source.GetElementType();
            var basictype = _Context.IsTypeBasic(type) ? type : null;

            var res = new JSArray(source.Cast<object>().Select(s => Map(s)), source, basictype);
            _Cacher.Cache(source, res);
            return res;
        }
    }
}
