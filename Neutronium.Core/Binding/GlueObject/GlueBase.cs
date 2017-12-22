using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject
{
    public abstract class GlueBase
    {
        public IJavascriptObject JsValue { get; private set; }

        private uint _Count = 0;

        public IJsCsGlue AddRef()
        {
            _Count++;
            return (IJsCsGlue)this;
        }

        public bool Release()
        {
            return (--_Count == 0);
        }

        protected void SetJsValue(IJavascriptObject value)
        {
            JsValue = value;
        }

        protected abstract void ComputeString(IDescriptionBuilder context);

        public void BuilString(IDescriptionBuilder context)
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
