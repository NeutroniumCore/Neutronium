using System;
using Tests.Infra.WebBrowserEngineTesterHelper.Windowless;

namespace Tests.Infra.WebBrowserEngineTesterHelper.Context 
{
    public interface IBasicWindowLessHTMLEngineProvider : IDisposable 
    {
        IWindowlessHTMLEngineBuilder GetWindowlessEnvironment();
    }
}
