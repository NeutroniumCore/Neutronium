using System.Collections.Generic;
using System.Text;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core.HTMLBinding
{
    public interface IJSCSGlue
    {
        IJavascriptObject JSValue { get; }

        object CValue { get;}

        JSCSGlueType Type { get; }

        IEnumerable<IJSCSGlue> GetChildren();

        void BuilString(StringBuilder sb, HashSet<IJSCSGlue> alreadyComputed);
    }
}
