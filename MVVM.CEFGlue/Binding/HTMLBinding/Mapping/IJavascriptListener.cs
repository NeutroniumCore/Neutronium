using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MVVM.CEFGlue.Binding.HTMLBinding.V8JavascriptObject;


namespace MVVM.CEFGlue.HTMLBinding
{
    public interface IJavascriptListener
    {
        void OnJavaScriptObjectChanges(IJavascriptObject objectchanged, string PropertyName, IJavascriptObject newValue);

        void OnJavaScriptCollectionChanges(IJavascriptObject collectionchanged, IJavascriptObject[] value, IJavascriptObject[] status, IJavascriptObject[] index);
    }
}
