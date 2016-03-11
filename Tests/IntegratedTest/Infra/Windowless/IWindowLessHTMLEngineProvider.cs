using System;

namespace IntegratedTest.Infra.Windowless 
{
    public interface IWindowLessHTMLEngineProvider : IDisposable 
    {
        WindowlessTestEnvironment GetWindowlessEnvironment();
    }
}
