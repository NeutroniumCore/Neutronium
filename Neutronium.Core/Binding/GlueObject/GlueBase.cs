using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System.Collections.Generic;
using System.Text;

namespace Neutronium.Core.Binding.GlueObject
{
    public abstract class GlueBase
    {
        protected abstract void ComputeString(StringBuilder sb, HashSet<IJSCSGlue> alreadyComputed);

        protected abstract bool LocalComputeJavascriptValue(IJavascriptObjectFactory context);

        public abstract IEnumerable<IJSCSGlue> GetChildren();

        protected virtual void AfterChildrenComputeJavascriptValue()
        {
        }

        public void BuilString(StringBuilder sb, HashSet<IJSCSGlue> alreadyComputed)
        {
            if (!alreadyComputed.Add(this as IJSCSGlue))
                return;

            ComputeString(sb, alreadyComputed);
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
            var sb = new StringBuilder();
            BuilString(sb, new HashSet<IJSCSGlue>());
            return sb.ToString();
        }
    }
}
