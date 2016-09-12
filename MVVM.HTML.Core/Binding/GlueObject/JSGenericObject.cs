using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neutronium.Core.JavascriptEngine.JavascriptObject;
using Neutronium.Core.JavascriptFramework;

namespace Neutronium.Core.Binding.GlueObject
{
    public class JsGenericObject : GlueBase, IJSObservableBridge
    {
        private readonly HTMLViewContext _HTMLViewContext;     
        private IJavascriptObject _MappedJSValue;
        private readonly Dictionary<string, IJSCSGlue> _Attributes = new Dictionary<string, IJSCSGlue>();

        public IReadOnlyDictionary<string, IJSCSGlue> Attributes => _Attributes;
        public IJavascriptObject JSValue { get; }
        public IJavascriptObject MappedJSValue => _MappedJSValue;
        public object CValue { get; }
        public JsCsGlueType Type => JsCsGlueType.Object;
        private IJavascriptViewModelUpdater ViewModelUpdater => _HTMLViewContext.ViewModelUpdater;

        public JsGenericObject(HTMLViewContext context, IJavascriptObject value, object icValue)
        {
            JSValue = value;
            CValue = icValue;
            _HTMLViewContext = context;
        }

        private JsGenericObject(HTMLViewContext context, IJavascriptObject value)
        {
            JSValue = value;
            _MappedJSValue = value;
            CValue = null;
            _HTMLViewContext = context;
        }

        public static JsGenericObject CreateNull(HTMLViewContext context)
        {
            return new JsGenericObject(context, context.WebView.Factory.CreateNull());
        }

        protected override void ComputeString(StringBuilder sb, HashSet<IJSCSGlue> alreadyComputed)
        {
            sb.Append("{");

            var first = true;
            foreach (var it in _Attributes.Where(kvp => kvp.Value.Type != JsCsGlueType.Command))
            {
                if (!first)
                    sb.Append(",");

                sb.Append($@"""{it.Key}"":");

                first = false;
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
            ViewModelUpdater.UpdateProperty(_MappedJSValue, propertyName, glue.GetJSSessionValue());
        }    
    }
}
