using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using MVVM.Component;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Extension;
using Neutronium.Core.JavascriptEngine.JavascriptObject;

namespace Neutronium.Core.Binding
{
    internal class CSharpToJavascriptConverter 
    {
        private readonly IJavascriptSessionCache _Cacher;
        private readonly IJSCommandFactory _CommandFactory;
        private readonly IWebSessionLogger _Logger;
        private readonly HTMLViewContext _Context;

        public CSharpToJavascriptConverter(HTMLViewContext context, IJavascriptSessionCache icacher, IJSCommandFactory commandFactory, IWebSessionLogger logger)
        {
            _CommandFactory = commandFactory;
            _Logger = logger;
            _Context = context;
            _Cacher = icacher;
        }

        public IJSCSGlue Map(object ifrom, object iadditional = null)
        {
            return _Context.WebView.Evaluate(() => UnsafelMap(ifrom, iadditional));
        }

        public IJSCSGlue UnsafelMap(object from, object iadditional=null)
        {
            if (from == null)
                return JsGenericObject.CreateNull(_Context);

            var res = _Cacher.GetCached(from);
            if (res != null)
                return res;

            var command = from as ICommand;
            if (command != null)
                return _CommandFactory.Build(command);

            var simpleCommand = from as ISimpleCommand;
            if (simpleCommand != null)
                return _CommandFactory.Build(simpleCommand);

            var resultCommand = from as IResultCommand;
            if (resultCommand != null)
                return _CommandFactory.Build(resultCommand);

            IJavascriptObject value;
            if (_Context.WebView.Factory.CreateBasic(from, out value))
                return new JSBasicObject(value, from);

            if (from.GetType().IsEnum)
            {
                var trueres = new JSBasicObject(_Context.WebView.Factory.CreateEnum((Enum)from), from);
                _Cacher.CacheLocal(from, trueres);
                return trueres;
            }

            var ienfro = from as IEnumerable;
            if (ienfro!=null)
                return  Convert(ienfro);

            var resobject = _Context.WebView.Factory.CreateObject(true);

            var gres = new JsGenericObject(_Context, resobject, from);
            _Cacher.Cache(from, gres);

            MappNested(from, resobject,gres);
            MappNested(iadditional, resobject, gres);

            return gres;
        }

        private void MappNested(object from, IJavascriptObject resobject, JsGenericObject gres)
        {
            if (from == null)
                return;

           var propertyInfos = from.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead);

            foreach (var propertyInfo in propertyInfos)
            {
                var propertyName = propertyInfo.Name;
                object childvalue;
                try
                {
                    childvalue = propertyInfo.GetValue(from, null); 
                }
                catch(Exception e)
                {
                    _Logger.Info(()=> $"Unable to convert property {propertyName} from {@from} exception {e}");
                    continue;
                }

                var childres = UnsafelMap(childvalue);          
                _Context.WebView.Run(() => resobject.SetValue(propertyName, childres.JSValue));
                gres.UpdateCSharpProperty(propertyName, childres);
            }
        }

        private IJSCSGlue Convert(IEnumerable source)
        {
            var res = new JSArray(_Context, source.Cast<object>().Select(s => UnsafelMap(s)), source);
            _Cacher.Cache(source, res);
            return res;
        }
    }
}
