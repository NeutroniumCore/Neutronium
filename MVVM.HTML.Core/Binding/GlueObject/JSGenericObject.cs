using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.V8JavascriptObject;
using MVVM.HTML.Core.Binding.Mapping;

namespace MVVM.HTML.Core.HTMLBinding
{
    public class JSGenericObject : GlueBase, IJSObservableBridge
    {
        private IWebView _WebView;
        public JSGenericObject(IWebView context, IJavascriptObject value, object icValue)
        {
            JSValue = value;
            CValue = icValue;
            _WebView = context;
        }

        private JSGenericObject(IWebView context, IJavascriptObject value)
        {
            JSValue = value;
            _MappedJSValue = value;
            CValue = null;
            _WebView = context;
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
        public IReadOnlyDictionary<string, IJSCSGlue> Attributes { get { return _Attributes; } }

        public IJavascriptObject JSValue { get; private set; }

        private IJavascriptObject _MappedJSValue;
        public IJavascriptObject MappedJSValue { get { return _MappedJSValue; } }

        public void SetMappedJSValue(IJavascriptObject ijsobject, IJavascriptToCSharpConverter mapper)
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

        public void UpdateCSharpProperty(string PropertyName, IJSCSGlue glue)
        {
            _Attributes[PropertyName] = glue;
        }

        public void Reroot(string PropertyName, IJSCSGlue newValue)
        { 
            UpdateCSharpProperty(PropertyName, newValue);

            IJavascriptObject silenter = null;
            if ( _Silenters.TryGetValue(PropertyName,out silenter))
            {
                silenter.InvokeAsync("silent", _WebView, newValue.GetJSSessionValue());      
            }
            else
            {
                _WebView.RunAsync( ()=>
                    {
                        silenter = _Silenters.FindOrCreateEntity(PropertyName, name => 
                            _MappedJSValue.GetValue(name));

                        silenter.Invoke("silent", _WebView, newValue.GetJSSessionValue());
                    });
            }
        }     
    }
}
