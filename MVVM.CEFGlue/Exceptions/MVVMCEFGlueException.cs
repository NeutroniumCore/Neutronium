using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVVM.CEFGlue.Exceptions
{
    public class MVVMCefGlueArgumentException : ArgumentException
    {
        public MVVMCefGlueArgumentException(string iM)
            : base(iM)
        {
        }
    }

    public class MVVMCEFGlueException : Exception
    {
        public MVVMCEFGlueException(string iM)
            : base(iM)
        {
        }
    }
    
}
