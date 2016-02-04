using Xilium.CefGlue;

namespace MVVM.Cef.Glue.CefSession
{
    public static class CefCoreSessionSingleton
    {
        public static ICefCoreSession Session
        {
            get;
            internal set;
        }

        public static CefCoreSessionFactory SessionFactory
        {
            get;
            private set;
        }

        public static ICefCoreSession GetAndInitIfNeeded(CefSettings iCefSettings = null, params string[] args)
        {
            if (Session==null)
            {
                if (SessionFactory==null)
                    SessionFactory = new CefCoreSessionFactory(iCefSettings, args);

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
