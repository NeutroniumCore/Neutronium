using System;

namespace Tests.Infra.IntegratedContextTesterHelper.Windowless 
{
    public interface IWindowLessHTMLEngineProvider : IDisposable 
    {
        IWindowlessIntegratedContextBuilder GetWindowlessEnvironment();
    }
}
