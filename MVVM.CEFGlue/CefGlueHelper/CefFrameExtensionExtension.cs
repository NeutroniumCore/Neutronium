using MVVM.CEFGlue.CefSession;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace MVVM.CEFGlue.CefGlueHelper
{
    public static class CefFrameExtensionExtension
    {
        public static CefV8Context GetMainContext(this CefFrame @this)
        {
            return CefCoreSessionSingleton.Session.CefApp.GetContext(@this);
        }

    }
}
