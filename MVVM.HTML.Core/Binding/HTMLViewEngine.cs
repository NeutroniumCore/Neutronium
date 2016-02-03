using MVVM.HTML.Core.JavascriptEngine;
using MVVM.HTML.Core.V8JavascriptObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.HTML.Core.Binding
{
    public class HTMLViewEngine
    {
        internal HTMLViewEngine(IHTMLWindowProvider hTMLWindowProvider, IJavascriptSessionInjectorFactory sessionInjectorFactory)
        {
            HTMLWindowProvider = hTMLWindowProvider;
            SessionInjectorFactory = sessionInjectorFactory;
        }

        public IHTMLWindowProvider HTMLWindowProvider { get; private set; }

        public IJavascriptSessionInjectorFactory SessionInjectorFactory { get; private set; }
    }
}
