using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Windows.Input;
using System.Diagnostics;

using MVVM.HTML.Core.Binding;
using MVVM.Component;
using MVVM.HTML.Core.V8JavascriptObject;

namespace MVVM.HTML.Core.HTMLBinding
{
    internal class CSharpToJavascriptMapper 
    {
        private readonly IJSCBridgeCache _Cacher;
        private readonly HTMLViewContext _Context;

        public CSharpToJavascriptMapper(HTMLViewContext context, IJSCBridgeCache icacher)
        {
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

            IJSCSGlue res = null;
            res = _Cacher.GetCached(ifrom);
            if (res != null)
            {
                return res;
            }

            if (ifrom is ICommand)
                return new JSCommand(_Context.WebView, _Context.UIDispatcher, ifrom as ICommand);

            if (ifrom is ISimpleCommand)
                return new JSSimpleCommand(_Context.WebView, ifrom as ISimpleCommand);

            if (ifrom is IResultCommand)
                return new JSResultCommand(_Context.WebView, ifrom as IResultCommand);

            IJavascriptObject value;
            if (_Context.WebView.Factory.SolveBasic(ifrom, out value))
            {
                return new JSBasicObject(value, ifrom);
            }

            if (ifrom.GetType().IsEnum)
            {
                var trueres = new JSBasicObject(_Context.WebView.Factory.CreateEnum((Enum)ifrom), ifrom);
                _Cacher.CacheLocal(ifrom, trueres);
                return trueres;
            }

            IEnumerable ienfro = ifrom as IEnumerable;
            if ((ienfro!=null) && Convert(ienfro, out res))
            {
                return res;
            }

            IJavascriptObject resobject = _Context.WebView.Factory.CreateObject(true);

            JSGenericObject gres = new JSGenericObject(_Context.WebView, resobject, ifrom);
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

                gres.Attributes[pn] = childres;
            }

            return gres;
        }      
 
        private bool Convert(IEnumerable source, out IJSCSGlue res)
        {
            res = new JSArray(this._Context.WebView, _Context.UIDispatcher, source.Cast<object>().Select(s => Map(s)), source);
            _Cacher.Cache(source, res);
            return true;
        }
    }
}
