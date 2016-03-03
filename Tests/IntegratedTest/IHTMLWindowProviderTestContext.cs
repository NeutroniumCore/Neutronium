using System;
using IntegratedTest.WPF.Infra;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace IntegratedTest 
{
    public interface IHTMLWindowProviderTestContext : IDisposable 
    {
        IJavascriptUIFrameworkManager GetUIFrameworkManager();

        WindowTestEnvironment GetWindowEnvironment();

        WindowlessTestEnvironment GetWindowlessEnvironment();
    }
}
