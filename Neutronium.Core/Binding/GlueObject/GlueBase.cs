using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject
{
    public abstract class GlueBase
    {
        public IJavascriptObject JsValue { get; private set; }
        public bool Marked { get; set; } = true;

        public void SetJsValue(IJavascriptObject value)
        {
            JsValue = value;
        }

        protected abstract void ComputeString(DescriptionBuilder context);

        public void BuilString(DescriptionBuilder context)
        {
            var contextualName = context.GetContextualName(this as IJsCsGlue);
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
