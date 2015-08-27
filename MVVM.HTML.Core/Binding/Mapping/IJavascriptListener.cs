using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MVVM.HTML.Core.V8JavascriptObject;


namespace MVVM.HTML.Core.HTMLBinding
{
    public interface IJavascriptListener
    {
        void OnJavaScriptObjectChanges(IJavascriptObject objectchanged, string PropertyName, IJavascriptObject newValue);

        void OnJavaScriptCollectionChanges(IJavascriptObject collectionchanged, IJavascriptObject[] value, IJavascriptObject[] status, IJavascriptObject[] index);
    }
}
