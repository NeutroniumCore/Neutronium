using System;

namespace MVVM.HTML.Core.Exceptions
{
    public static class ExceptionHelper
    {
        public static Exception Get(string iMessage)
        {
            return new MVVMCEFGlueException(iMessage);
        }

        public static Exception GetUnexpected()
        {
            return Get("Unexpected error");
        }

        public static ArgumentException GetArgument(string iMessage)
        {
            return new MVVMCefGlueArgumentException(iMessage);
        }
    }
}
