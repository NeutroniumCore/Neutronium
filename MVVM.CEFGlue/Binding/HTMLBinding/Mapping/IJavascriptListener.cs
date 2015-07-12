using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xilium.CefGlue;

namespace MVVM.CEFGlue.HTMLBinding
{
    public interface IJavascriptListener
    {
        void OnJavaScriptObjectChanges(CefV8Value objectchanged, string PropertyName, CefV8Value newValue);

        void OnJavaScriptCollectionChanges(CefV8Value collectionchanged, CefV8Value[] value, CefV8Value[] status, CefV8Value[] index);
    }
}
