using CefGlue.Window;
using MVVM.CEFGlue.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace MVVM.CEFGlue.CefSession
{
    public static class CefCoreSessionSingleton
    {
        public static CefCoreSession Session
        {
            get;
            private set;
        }

        public static CefCoreSessionFactory SessionFactory
        {
            get;
            private set;
        }

        public static CefCoreSession GetAndInitIfNeeded(IUIDispatcher iIUIDispatcher=null, CefSettings iCefSettings = null, params string[] args)
        {
            if (Session==null)
            {
                if (SessionFactory==null)
                    SessionFactory = new CefCoreSessionFactory(iIUIDispatcher??new NullUIDispatcher(),iCefSettings, args);

                Session = SessionFactory.GetSession();
            }

            return Session;
        }

        public static void Clean()
        {
            if (Session == null)
                return;
           
            Session.Dispose();
            Session = null;
        }

    }
}
