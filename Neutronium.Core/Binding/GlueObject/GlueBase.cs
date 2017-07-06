using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.GlueObject
{
    public abstract class GlueBase
    {
        public IJavascriptObject JSValue { get; private set; }

        public abstract IEnumerable<IJSCSGlue> GetChildren();

        public void SetJSValue(IJavascriptObject value)
        {
            JSValue = value;
        }

        protected abstract void ComputeString(DescriptionBuilder context);

        public void BuilString(DescriptionBuilder context)
        {
            var contextualName = context.GetContextualName(this as IJSCSGlue);
            if (contextualName != null)
            {
                context.Append(contextualName);
                return;
            }
            ComputeString(context);
        }

        public override string ToString()
        {
            var context = new DescriptionBuilder();
            BuilString(context);
            return context.BuildString();
        }
    }
}
