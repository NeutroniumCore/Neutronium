using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Windows.Input;
using System.Diagnostics;

using MVVM.HTML.Core.Infra;

using MVVM.Component;
using MVVM.HTML.Core.V8JavascriptObject;
using MVVM.HTML.Core.Window;


namespace MVVM.HTML.Core.HTMLBinding
{
    internal class CSharpToJavascriptMapper 
    {
        private readonly IJSCBridgeCache _Cacher;
        private IWebView _IWebView;
        private IDispatcher _UIDispatcher;

        public CSharpToJavascriptMapper(IWebView context, IDispatcher iUIDispatcher, IJSCBridgeCache icacher)
        {
            _UIDispatcher = iUIDispatcher;
            _IWebView = context;
            _Cacher = icacher;
        }

        internal IJSCSGlue Map(object ifrom, object iadditional = null)
        {
            return _IWebView.Evaluate(() => InternalMap(ifrom, iadditional));
        }

        private IJSCSGlue InternalMap(object ifrom, object iadditional=null)
        {
            if (ifrom == null)
                return JSGenericObject.CreateNull(_IWebView);

            IJSCSGlue res = null;
            res = _Cacher.GetCached(ifrom);
            if (res != null)
            {
                return res;
            }

            if (ifrom is ICommand)
                return new JSCommand(_IWebView, _UIDispatcher, ifrom as ICommand);

            if (ifrom is ISimpleCommand)
                return new JSSimpleCommand(_IWebView, ifrom as ISimpleCommand);

            if (ifrom is IResultCommand)
                return new JSResultCommand(_IWebView, ifrom as IResultCommand);

            IJavascriptObject value;
            if (_IWebView.Factory.SolveBasic(ifrom, out value))
            {
                return new JSBasicObject(value, ifrom);
            }

            if (ifrom.GetType().IsEnum)
            {
                var trueres = new JSBasicObject(_IWebView.Factory.CreateEnum((Enum)ifrom), ifrom);
                _Cacher.CacheLocal(ifrom, trueres);
                return trueres;
            }

            IEnumerable ienfro = ifrom as IEnumerable;
            if ((ienfro!=null) && Convert(ienfro, out res))
            {
                return res;
            }

            IJavascriptObject resobject = _IWebView.Factory.CreateObject();

            JSGenericObject gres = new JSGenericObject(_IWebView,resobject, ifrom);
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

                _IWebView.Run(() => resobject.SetValue(pn, childres.JSValue));

                gres.Attributes[pn] = childres;
            }

            return gres;
        }      
 
        private bool Convert(IEnumerable source, out IJSCSGlue res)
        {
            res = new JSArray(this._IWebView, _UIDispatcher, source.Cast<object>().Select(s => Map(s)), source);
            _Cacher.Cache(source, res);
            return true;
        }
    }
}
