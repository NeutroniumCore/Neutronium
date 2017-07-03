using MoreCollection.Extensions;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.GlueObject
{
    public abstract class GlueBase
    {
        protected abstract void ComputeString(DescriptionBuilder context);

        protected abstract bool LocalComputeJavascriptValue(IWebView webView);

        public abstract IEnumerable<IJSCSGlue> GetChildren();

        protected virtual void AfterChildrenComputeJavascriptValue(IWebView webView)
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

        public void ComputeJavascriptValue(IWebView webView, IJavascriptSessionCache cache)
        {
            if (LocalComputeJavascriptValue(webView))
            {
                GetChildren().ForEach(child => child.ComputeJavascriptValue(webView, cache));
                AfterChildrenComputeJavascriptValue(webView);
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
