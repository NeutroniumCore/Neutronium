using System;
using Tests.Infra.HTMLEngineTesterHelper.Windowless;

namespace Tests.Infra.HTMLEngineTesterHelper.Context 
{
    public interface IBasicWindowLessHTMLEngineProvider : IDisposable 
    {
        IWindowlessHTMLEngineBuilder GetWindowlessEnvironment();
    }
}
