using System;

namespace MVVM.HTML.Core.Exceptions
{
    public class MVVMCefGlueArgumentException : ArgumentException
    {
        public MVVMCefGlueArgumentException(string iM) : base(iM)
        {
        }
    }

    public class MVVMCEFGlueException : Exception
    {
        public MVVMCEFGlueException(string iM) : base(iM)
        {
        }
    }
}
