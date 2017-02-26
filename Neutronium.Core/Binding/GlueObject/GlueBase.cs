using MoreCollection.Extensions;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.GlueObject
{
    public abstract class GlueBase
    {
        protected abstract void ComputeString(NameContext context);

        protected abstract bool LocalComputeJavascriptValue(IJavascriptObjectFactory context);

        public abstract IEnumerable<IJSCSGlue> GetChildren();

        protected virtual void AfterChildrenComputeJavascriptValue()
        {
        }

        public void BuilString(NameContext context)
        {
            if (!context.AlreadyComputed(this as IJSCSGlue))
                return;

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
            var context = new NameContext();
            BuilString(context);
            return context.BuildString();
        }
    }
}
