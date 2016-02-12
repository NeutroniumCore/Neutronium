using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Windows.Input;
using System.Diagnostics;

using MVVM.HTML.Core.Binding;
using MVVM.Component;
using MVVM.HTML.Core.Binding.Extension;
using MVVM.HTML.Core.Binding.GlueObject;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core.HTMLBinding
{
    internal class CSharpToJavascriptConverter 
    {
        private readonly IJavascriptSessionCache _Cacher;
        private readonly IJSCommandFactory _CommandFactory;
        private readonly HTMLViewContext _Context;

        public CSharpToJavascriptConverter(HTMLViewContext context, IJavascriptSessionCache icacher, IJSCommandFactory commandFactory)
        {
            _CommandFactory = commandFactory;
            _Context = context;
            _Cacher = icacher;
        }

        internal IJSCSGlue Map(object ifrom, object iadditional = null)
        {
            return _Context.WebView.Evaluate(() => InternalMap(ifrom, iadditional));
        }

        private IJSCSGlue InternalMap(object from, object iadditional=null)
        {
            if (from == null)
                return JSGenericObject.CreateNull(_Context.WebView);

            var res = _Cacher.GetCached(from);
            if (res != null)
                return res;

            var command = from as ICommand;
            if (command != null)
                return _CommandFactory.Build(_Context.WebView, _Context.UIDispatcher, command);

            var simpleCommand = from as ISimpleCommand;
            if (simpleCommand != null)
                return _CommandFactory.Build(_Context.WebView, _Context.UIDispatcher, simpleCommand);

            var resultCommand = from as IResultCommand;
            if (resultCommand != null)
                return _CommandFactory.Build(_Context.WebView, _Context.UIDispatcher, resultCommand);

            IJavascriptObject value;
            if (_Context.WebView.Factory.SolveBasic(from, out value))
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

            var gres = new JSGenericObject(_Context.WebView, resobject, from);
            _Cacher.Cache(from, gres);

            MappNested(from, resobject,gres);
            MappNested(iadditional, resobject, gres);

            return gres;
        }

        private void MappNested(object from, IJavascriptObject resobject, JSGenericObject gres)
        {
            if (from == null)
                return;

            IEnumerable<PropertyInfo> propertyInfos = from.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo propertyInfo in propertyInfos.Where(p => p.CanRead))
            {
                string pn = propertyInfo.Name;
                object childvalue;
                try
                {
                    childvalue = propertyInfo.GetValue(from, null); 
                }
                catch(Exception e)
                {
                    Trace.WriteLine(string.Format("MVVM for HTML: Unable to convert property {0} from {1} exception {2}", pn, from, e));
                    continue;
                }

                IJSCSGlue childres = InternalMap(childvalue);

                _Context.WebView.Run(() => resobject.SetValue(pn, childres.JSValue));

                gres.UpdateCSharpProperty(pn,childres);
            }
        }

        private IJSCSGlue Convert(IEnumerable source)
        {
            var res = new JSArray(this._Context.WebView, _Context.UIDispatcher, source.Cast<object>().Select(s => Map(s)), source);
            _Cacher.Cache(source, res);
            return res;
        }
    }
}
