using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Windows.Input;
using System.Diagnostics;

using Xilium.CefGlue;

using MVVM.CEFGlue.Infra;
using MVVM.CEFGlue.CefGlueHelper;
using MVVM.Component;
using MVVM.CEFGlue.Binding.HTMLBinding.V8JavascriptObject;


namespace MVVM.CEFGlue.HTMLBinding
{
    internal class CSharpToJavascriptMapper 
    {
        private readonly IJSOLocalBuilder _IJSOBuilder;
        private readonly IJSCBridgeCache _Cacher;
        private readonly BasicCSharpToJavascriptConverter _Basic;
        private IWebView _CefV8Context;
        public CSharpToJavascriptMapper(IWebView context, IJSOLocalBuilder Builder, IJSCBridgeCache icacher)
        {
            _CefV8Context = context;
            _IJSOBuilder = Builder;
            _Cacher = icacher;
            _Basic = new BasicCSharpToJavascriptConverter(_CefV8Context);
        }

        internal IJSCSGlue Map(object ifrom, object iadditional = null)
        {
            return _CefV8Context.Evaluate(() => InternalMap(ifrom, iadditional));
        }

        private IJSCSGlue InternalMap(object ifrom, object iadditional=null)
        {
            if (ifrom == null)
                return JSGenericObject.CreateNull(_CefV8Context, _IJSOBuilder);

            IJSCSGlue res = null;
            res = _Cacher.GetCached(ifrom);
            if (res != null)
            {
                return res;
            }

            if (ifrom is ICommand)
                return new JSCommand(_CefV8Context, _IJSOBuilder, ifrom as ICommand);

            if (ifrom is ISimpleCommand)
                return new JSSimpleCommand(_CefV8Context, _IJSOBuilder, ifrom as ISimpleCommand);

            if (ifrom is IResultCommand)
                return new JSResultCommand(_CefV8Context, _IJSOBuilder, ifrom as IResultCommand);

            CefV8Value value;
            if (_Basic.Solve(ifrom, out value))
            {
                return new JSBasicObject(value, ifrom);
            }

            if (ifrom.GetType().IsEnum)
            {
                var trueres = new JSBasicObject(_IJSOBuilder.CreateEnum((Enum)ifrom), ifrom);
                _Cacher.CacheLocal(ifrom, trueres);
                return trueres;
            }

            IEnumerable ienfro = ifrom as IEnumerable;
            if ((ienfro!=null) && Convert(ienfro, out res))
            {
                return res;
            }

            CefV8Value resobject = _IJSOBuilder.CreateJSO();

            JSGenericObject gres = new JSGenericObject(_CefV8Context,resobject, ifrom);
            _Cacher.Cache(ifrom, gres);

            MappNested(ifrom, resobject,gres);
            MappNested(iadditional, resobject, gres);

            return gres;
        }

        private JSGenericObject MappNested(object ifrom, CefV8Value resobject, JSGenericObject gres)
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
                    Trace.WriteLine(string.Format("MVVM for awesomium: Unable to convert property {0} from {1} exception {2}", pn, ifrom, e));
                    continue;
                }

                IJSCSGlue childres = InternalMap(childvalue);

                _CefV8Context.Run(() => resobject.SetValue(pn, childres.JSValue, CefV8PropertyAttribute.None));

                gres.Attributes[pn] = childres;
            }

            return gres;
        }      
 
        private bool Convert(IEnumerable source, out IJSCSGlue res)
        {
            res = new JSArray(this._CefV8Context, source.Cast<object>().Select(s => Map(s)), source, _Basic.GetElementType(source));
            _Cacher.Cache(source, res);
            return true;
        }
    }
}
