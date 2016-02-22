using System;
using System.Diagnostics;

namespace MVVM.HTML.Core.Exceptions
{
    public static class ExceptionHelper
    {
        private const string _Header = "MVVM for CEFGlue";

        public static void Log(string iMessageLog)
        {
            Trace.WriteLine(string.Format("{0} - {1}", _Header,iMessageLog));
        }
    
        public static Exception Get(string iMessage)
        {
            return new MVVMCEFGlueException(iMessage);
        }

        public static Exception GetUnexpected()
        {
            return Get("Unexpected error");
        }

        //static public Exception NoKoExtension()
        //{
        //   return Get("Critical error! You should add a link to knockout_Extension.js script to the HML document!");
        //}

        public static ArgumentException GetArgument(string iMessage)
        {
            return new MVVMCefGlueArgumentException(iMessage);
        }
    }
}
