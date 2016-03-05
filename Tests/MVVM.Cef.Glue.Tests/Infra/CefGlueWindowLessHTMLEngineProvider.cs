using System;
using IntegratedTest;

namespace MVVM.Cef.Glue.Tests.Infra 
{
    public class CefGlueWindowLessHTMLEngineProvider : IWindowLessHTMLEngineProvider 
    {
        public void Dispose() 
        {
            throw new NotImplementedException();
        }

        public WindowlessTestEnvironment GetWindowlessEnvironment() 
        {
            throw new NotImplementedException();
        }
    }
}
