using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using Neutronium.Core;
using Tests.Infra.IntegratedContextTesterHelper.Window;
using Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext;
using Tests.Infra.WebBrowserEngineTesterHelper.Window;
using HTMLControlBase = Neutronium.WPF.Internal.HTMLControlBase;
using Neutronium.Core.Log;
using Tests.Infra.WebBrowserEngineTesterHelper.Windowless;
using Xunit.Abstractions;

namespace Tests.Universal.NavigationTests
{
    public abstract class WpfComponentTest_Base<T>  where T : HTMLControlBase
    {
        protected readonly WindowTestContext _WindowTestEnvironment;
        protected readonly IWebSessionLogger _Logger;

        protected WpfComponentTest_Base(IWindowContextProvider windowTestEnvironment, ITestOutputHelper testOutputHelper) 
        {
            _Logger = new TestLogger(testOutputHelper).Add(new BasicLogger());
            _WindowTestEnvironment = windowTestEnvironment.WindowTestContext;
        }

        protected string GetPath(TestContext context) 
        {
             return _WindowTestEnvironment.HtmlProvider.GetHtmlPath(context);
        }

        protected string GetRelativePath(TestContext context)
        {
            return _WindowTestEnvironment.HtmlProvider.GetRelativeHtmlPath(context);
        }

        private WindowTest BuildWindow(Func<T> iWebControlFac, bool iManageLifeCycle)
        {
            return new WindowTest(_WindowTestEnvironment,
                (w) =>
                {
                    var stackPanel = new StackPanel() 
                    {
                        Height = 400
                    };
                    w.Content = stackPanel;
                    var iWebControl = iWebControlFac();
                    iWebControl.Height = 400;
                    if (iManageLifeCycle)
                        w.Closed += (o, e) => { iWebControl.Dispose(); };
                    stackPanel.Children.Add(iWebControl);
                });
        }

        protected abstract T GetNewHTMLControlBase(bool iDebug);

        private class HTMLControlBase_Handler<Thandler> 
        {
            public Thandler Handler { get; set; }
        }

        private WindowTest InitTest(HTMLControlBase_Handler<T> handler, bool debug = false, bool manageLifeCycle = true)
        {
            AssemblyHelper.SetEntryAssembly();

            T WebControlFac()
            {
                var element = GetNewHTMLControlBase(debug);
                element.WebSessionLogger = _Logger;
                return handler.Handler = element;
            }

            return BuildWindow(WebControlFac, manageLifeCycle);
        }

        internal void Test(Action<T> test, bool debug = false, bool manageLifeCycle = true) 
        {
            var handler = new HTMLControlBase_Handler<T>();
            using (var wcontext = InitTest(handler, debug, manageLifeCycle))
            {
                wcontext.RunOnUIThread( () => test(handler.Handler)).Wait();
            }
        }

        internal async Task Test(Func<T, WindowTest, Task> test, bool debug = false, bool manageLifeCycle = true) 
        {
            var handler = new HTMLControlBase_Handler<T>();
            using (var wcontext = InitTest(handler, debug, manageLifeCycle)) 
            {
                await wcontext.RunOnUIThread(async () => await test(handler.Handler, wcontext));
            }
        }
    }
}
