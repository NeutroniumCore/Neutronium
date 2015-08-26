using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MVVM.HTML.Core.V8JavascriptObject;


namespace MVVM.HTML.Core.HTMLBinding
{
    internal interface IJavascriptMapper
    {
        void RegisterFirst(IJavascriptObject iRoot);

        void RegisterMapping(IJavascriptObject iFather, string att, IJavascriptObject iChild);

        void RegisterCollectionMapping(IJavascriptObject iFather, string att, int index, IJavascriptObject iChild);

        void End(IJavascriptObject iRoot);
    }
}
