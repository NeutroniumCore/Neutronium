using System;
using Tests.Infra.HTMLEngineTesterHelper.Context;
using Tests.Infra.JavascriptEngineTesterHelper;

namespace Tests.Infra.IntegratedContextTesterHelper.Windowless 
{
    public interface IWindowLessHTMLEngineProvider
    {
        FrameworkTestContext FrameworkTestContext { get; }

        IBasicWindowLessHTMLEngineProvider WindowBuilder { get; }
    }
}
