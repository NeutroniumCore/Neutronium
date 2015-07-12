using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xilium.CefGlue;

namespace MVVM.CEFGlue.HTMLBinding
{
    public interface IJSObservableBridge : IJSCSGlue
    {
        CefV8Value MappedJSValue { get; }
        void SetMappedJSValue(CefV8Value ijsobject, IJSCBridgeCache mapper);
    }

}
