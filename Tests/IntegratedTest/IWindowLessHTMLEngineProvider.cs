using System;

namespace IntegratedTest 
{
    public interface IWindowLessHTMLEngineProvider : IDisposable 
    {
        WindowlessTestEnvironment GetWindowlessEnvironment();
    }
}
