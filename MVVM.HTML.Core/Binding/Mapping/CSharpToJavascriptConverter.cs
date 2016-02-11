using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Windows.Input;
using System.Diagnostics;

using MVVM.HTML.Core.Binding;
using MVVM.Component;
using MVVM.HTML.Core.Binding.GlueObject;
using MVVM.HTML.Core.V8JavascriptObject;

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

        private IJSCSGlue InternalMap(object ifrom, object iadditional=null)
        {
            if (ifrom == null)
                return JSGenericObject.CreateNull(_Context.WebView);

            var res = _Cacher.GetCached(ifrom);
            if (res != null)
                return res;

            var command = ifrom as ICommand;
            if (command != null)
                return _CommandFactory.Build(_Context.WebView, _Context.UIDispatcher, command);

            var simpleCommand = ifrom as ISimpleCommand;
            if (simpleCommand != null)
                return _CommandFactory.Build(_Context.WebView, _Context.UIDispatcher, simpleCommand);

            var resultCommand = ifrom as IResultCommand;
            if (resultCommand != null)
                return _CommandFactory.Build(_Context.WebView, _Context.UIDispatcher, resultCommand);

            IJavascriptObject value;
            if (_Context.WebView.Factory.SolveBasic(ifrom, out value))
                return new JSBasicObject(value, ifrom);

            if (ifrom.GetType().IsEnum)
            {
                var trueres = new JSBasicObject(_Context.WebView.Factory.CreateEnum((Enum)ifrom), ifrom);
                _Cacher.CacheLocal(ifrom, trueres);
                return trueres;
            }

            var ienfro = ifrom as IEnumerable;
            if (ienfro!=null)
                return  Convert(ienfro);

            var resobject = _Context.WebView.Factory.CreateObject(true);

            var gres = new JSGenericObject(_Context.WebView, resobject, ifrom);
            _Cacher.Cache(ifrom, gres);

            MappNested(ifrom, resobject,gres);
            MappNested(iadditional, resobject, gres);

            return gres;
        }

        private JSGenericObject MappNested(object ifrom, IJavascriptObject resobject, JSGenericObject gres)
        {
            if (ifrom == null)
                return gres;

            IEnumerable<PropertyInfo> propertyInfos = ifrom.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo propertyInfo in propertyInfos.Where(p => p.CanRead))
            {
                string pn = propertyInfo.Name;
                object childvalue = null;
                try
                {
                    childvalue = propertyInfo.GetValue(ifrom, null); 
                }
                catch(Exception e)
                {
                    Trace.WriteLine(string.Format("MVVM for HTML: Unable to convert property {0} from {1} exception {2}", pn, ifrom, e));
                    continue;
                }

                IJSCSGlue childres = InternalMap(childvalue);

                _Context.WebView.Run(() => resobject.SetValue(pn, childres.JSValue));

                gres.UpdateCSharpProperty(pn,childres);
            }

            return gres;
        }

        private IJSCSGlue Convert(IEnumerable source)
        {
            var res = new JSArray(this._Context.WebView, _Context.UIDispatcher, source.Cast<object>().Select(s => Map(s)), source);
            _Cacher.Cache(source, res);
            return res;
        }
    }
}
