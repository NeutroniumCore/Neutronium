using System;
using IntegratedTest.WPF.Infra;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace IntegratedTest 
{
    public interface IWindowLessHTMLEngineProvider : IDisposable 
    {
        IJavascriptUIFrameworkManager GetUIFrameworkManager();

        WindowlessTestEnvironment GetWindowlessEnvironment();
    }
}
