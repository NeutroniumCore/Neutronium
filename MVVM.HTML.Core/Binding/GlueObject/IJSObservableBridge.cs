using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MVVM.HTML.Core.V8JavascriptObject;

namespace MVVM.HTML.Core.HTMLBinding
{
    public interface IJSObservableBridge : IJSCSGlue
    {
        IJavascriptObject MappedJSValue { get; }
        void SetMappedJSValue(IJavascriptObject ijsobject, IJSCBridgeCache mapper);
    }

}
