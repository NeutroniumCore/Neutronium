using MoreCollection.Extensions;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.GlueObject
{
    public abstract class GlueBase
    {
        protected abstract void ComputeString(DescriptionBuilder context);

        protected abstract bool LocalComputeJavascriptValue(IJavascriptObjectFactory context);

        public abstract IEnumerable<IJSCSGlue> GetChildren();

        protected virtual void AfterChildrenComputeJavascriptValue()
        {
        }

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

        public void ComputeJavascriptValue(IJavascriptObjectFactory factory, IJavascriptSessionCache cache)
        {
            if (LocalComputeJavascriptValue(factory))
            {
                GetChildren().ForEach(child => child.ComputeJavascriptValue(factory, cache));
                AfterChildrenComputeJavascriptValue();
            }       
        }

        public override string ToString()
        {
            var context = new DescriptionBuilder();
            BuilString(context);
            return context.BuildString();
        }
    }
}
