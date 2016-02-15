using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.Binding.Mapping;

namespace MVVM.HTML.Core.HTMLBinding
{
    public class JSGenericObject : GlueBase, IJSObservableBridge
    {
        private readonly HTMLViewContext _HTMLViewContext;     
        private IJavascriptObject _MappedJSValue;
        private readonly Dictionary<string, IJSCSGlue> _Attributes = new Dictionary<string, IJSCSGlue>();
        private readonly IDictionary<string, IJavascriptObject> _Silenters = new Dictionary<string, IJavascriptObject>();

        public IReadOnlyDictionary<string, IJSCSGlue> Attributes { get { return _Attributes; } }
        public IJavascriptObject JSValue { get; private set; }
        public IJavascriptObject MappedJSValue { get { return _MappedJSValue; } }
        public object CValue { get; private set; }
        public JSCSGlueType Type { get { return JSCSGlueType.Object; } } 
        private IWebView WebView { get { return _HTMLViewContext.WebView; } }
        private IJavascriptSessionInjector Injector { get { return _HTMLViewContext.JavascriptSessionInjector; } }

        public JSGenericObject(HTMLViewContext context, IJavascriptObject value, object icValue)
        {
            JSValue = value;
            CValue = icValue;
            _HTMLViewContext = context;
        }

        private JSGenericObject(HTMLViewContext context, IJavascriptObject value)
        {
            JSValue = value;
            _MappedJSValue = value;
            CValue = null;
            _HTMLViewContext = context;
        }

        public static JSGenericObject CreateNull(HTMLViewContext context)
        {
            return new JSGenericObject(context, context.WebView.Factory.CreateNull());
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

        public void SetMappedJSValue(IJavascriptObject ijsobject)
        {
            _MappedJSValue = ijsobject;
        }

        public IEnumerable<IJSCSGlue> GetChildren()
        {
            return _Attributes.Values; 
        }

        public void UpdateCSharpProperty(string propertyName, IJSCSGlue glue)
        {
            _Attributes[propertyName] = glue;
        }

        public void ReRoot(string propertyName, IJSCSGlue glue)
        {
            UpdateCSharpProperty(propertyName, glue);
            Injector.UpdateProperty(_MappedJSValue, propertyName, glue.GetJSSessionValue());
        }    
    }
}
