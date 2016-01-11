using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.V8JavascriptObject;

namespace MVVM.HTML.Core.HTMLBinding
{
    public class JSGenericObject : GlueBase, IJSObservableBridge
    {
        private IWebView _CefV8Context;
        public JSGenericObject(IWebView context, IJavascriptObject value, object icValue)
        {
            JSValue = value;
            CValue = icValue;
            _CefV8Context = context;
        }

        private JSGenericObject(IWebView context, IJavascriptObject value)
        {
            JSValue = value;
            _MappedJSValue = value;
            CValue = null;
            _CefV8Context = context;
        }

        public static JSGenericObject CreateNull(IWebView context)
        {
            return new JSGenericObject(context, context.Factory.CreateNull());
        }

        protected override void ComputeString(StringBuilder sb, HashSet<IJSCSGlue> alreadyComputed)
        {
            sb.Append("{");

            bool f = true;
            foreach (var it in _Attributes.Where(kvp => kvp.Value.Type != JSCSGlueType.Command))
            {
                if (!f)
                    sb.Append(",");

                sb.Append(string.Format(@"""{0}"":", it.Key));

                f = false;
                it.Value.BuilString(sb, alreadyComputed);
            }

            sb.Append("}");
        }

        private Dictionary<string, IJSCSGlue> _Attributes = new Dictionary<string, IJSCSGlue>();

        public IDictionary<string, IJSCSGlue> Attributes { get { return _Attributes; } }

        public IJavascriptObject JSValue { get; private set; }

        private IJavascriptObject _MappedJSValue;

        public IJavascriptObject MappedJSValue { get { return _MappedJSValue; } }

        public void SetMappedJSValue(IJavascriptObject ijsobject, IJSCBridgeCache mapper)
        {
            _MappedJSValue = ijsobject;
        }

        private IDictionary<string, IJavascriptObject> _Silenters = new Dictionary<string, IJavascriptObject>(); 

        public object CValue { get; private set; }

        public JSCSGlueType Type { get { return JSCSGlueType.Object; } }

        public IEnumerable<IJSCSGlue> GetChildren()
        {
            return _Attributes.Values; 
        }

        public void UpdateCSharpProperty(string PropertyName, IJSCBridgeCache converter, IJavascriptObject newValue)
        {
            PropertyInfo propertyInfo = CValue.GetType().GetProperty(PropertyName, BindingFlags.Public | BindingFlags.Instance);
            if (!propertyInfo.CanWrite)
                return;

            var type = propertyInfo.PropertyType.GetUnderlyingNullableType() ?? propertyInfo.PropertyType;
            IJSCSGlue glue = converter.GetCachedOrCreateBasic(newValue, type);
            _Attributes[PropertyName] = glue;
            propertyInfo.SetValue(CValue, glue.CValue, null);
        }

        public void Reroot(string PropertyName, IJSCSGlue newValue)
        { 
            _Attributes[PropertyName]=newValue;

            IJavascriptObject silenter = null;
            if ( _Silenters.TryGetValue(PropertyName,out silenter))
            {
                silenter.InvokeAsync("silent", _CefV8Context, newValue.GetJSSessionValue());      
            }
            else
            {
                _CefV8Context.RunAsync( ()=>
                    {
                        var jso = _MappedJSValue;
                        if (!_Silenters.TryGetValue(PropertyName, out silenter))
                        {
                            silenter = jso.GetValue(PropertyName);
                            _Silenters.Add(PropertyName, silenter);
                        }
                        silenter.Invoke("silent", _CefV8Context, newValue.GetJSSessionValue());
                    });
            }
        }     
    }
}
