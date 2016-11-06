using System.Collections.Generic;
using System.Text;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject
{
    public interface IJSCSGlue
    {
        IJavascriptObject JSValue { get; }

        object CValue { get;}

        JsCsGlueType Type { get; }

        IEnumerable<IJSCSGlue> GetChildren();

        void BuilString(StringBuilder sb, HashSet<IJSCSGlue> alreadyComputed);

        void ComputeJavascriptValue(IJavascriptObjectFactory context, IJavascriptSessionCache cache);
    }
}
