using System.Collections.Generic;
using System.Linq;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using MoreCollection.Extensions;
using MoreCollection.Dictionary;

namespace Neutronium.Core.Binding.GlueObject
{
    public class JsGenericObject : GlueBase, IJSObservableBridge
    {
        private readonly HTMLViewContext _HTMLViewContext;     
        private IJavascriptObject _MappedJSValue;
        private readonly HybridDictionary<string, IJSCSGlue> _Attributes;

        public IReadOnlyDictionary<string, IJSCSGlue> Attributes => _Attributes;
        public IJavascriptObject JSValue { get; private set; }
        public IJavascriptObject MappedJSValue => _MappedJSValue;
        public object CValue { get; }
        public JsCsGlueType Type => JsCsGlueType.Object;
        private IJavascriptViewModelUpdater ViewModelUpdater => _HTMLViewContext.ViewModelUpdater;

        public JsGenericObject(HTMLViewContext context, object cValue, int childrenCount)
        {
            CValue = cValue;
            _HTMLViewContext = context;
            _Attributes = new HybridDictionary<string, IJSCSGlue>(childrenCount);
        }

        protected override bool LocalComputeJavascriptValue(IJavascriptObjectFactory factory)
        {
            if (JSValue != null)
                return false;

            JSValue = factory.CreateObject(true);       
            return true;
        }

        protected override void AfterChildrenComputeJavascriptValue()
        {
            _Attributes.ForEach(attribute => JSValue.SetValue(attribute.Key, attribute.Value.JSValue));
        }

        protected override void ComputeString(DescriptionBuilder context)
        {
            context.Append("{");
            var first = true;
            foreach (var it in _Attributes.Where(kvp => kvp.Value.Type != JsCsGlueType.Command)
                                          .OrderBy(kvp =>kvp.Key))
            {
                if (!first)
                    context.Append(",");

                context.Append($@"""{it.Key}"":");

                using (context.PushContext(it.Key))
                {                   
                    it.Value.BuilString(context);
                }

                first = false;           
            }

            context.Append("}");
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
