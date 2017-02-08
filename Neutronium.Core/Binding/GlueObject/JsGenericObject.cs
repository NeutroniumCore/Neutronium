using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using MoreCollection.Extensions;

namespace Neutronium.Core.Binding.GlueObject
{
    public class JsGenericObject : GlueBase, IJSObservableBridge
    {
        private readonly HTMLViewContext _HTMLViewContext;     
        private IJavascriptObject _MappedJSValue;
        private readonly Dictionary<string, IJSCSGlue> _Attributes = new Dictionary<string, IJSCSGlue>();

        public IReadOnlyDictionary<string, IJSCSGlue> Attributes => _Attributes;
        public IJavascriptObject JSValue { get; private set; }
        public IJavascriptObject MappedJSValue => _MappedJSValue;
        public object CValue { get; }
        public JsCsGlueType Type => JsCsGlueType.Object;
        private IJavascriptViewModelUpdater ViewModelUpdater => _HTMLViewContext.ViewModelUpdater;

        public JsGenericObject(HTMLViewContext context, object icValue)
        {
            CValue = icValue;
            _HTMLViewContext = context;
        }

        protected override bool LocalComputeJavascriptValue(IJavascriptObjectFactory factory)
        {
            if (JSValue != null)
                return false;

            if (CValue != null)
            {
                JSValue = factory.CreateObject(true);
            }
            else
            {
                JSValue = factory.CreateNull();
                _MappedJSValue = JSValue;
            }        
            return true;
        }

        protected override void AfterChildrenComputeJavascriptValue()
        {
            _Attributes.ForEach(attribute => JSValue.SetValue(attribute.Key, attribute.Value.JSValue));
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

        public override IEnumerable<IJSCSGlue> GetChildren()
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
