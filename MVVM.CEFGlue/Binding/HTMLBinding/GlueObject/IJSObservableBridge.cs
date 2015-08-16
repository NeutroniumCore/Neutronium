using MVVM.CEFGlue.Binding.HTMLBinding.V8JavascriptObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVVM.CEFGlue.HTMLBinding
{
    public interface IJSObservableBridge : IJSCSGlue
    {
        IJavascriptObject MappedJSValue { get; }
        void SetMappedJSValue(IJavascriptObject ijsobject, IJSCBridgeCache mapper);
    }

}
