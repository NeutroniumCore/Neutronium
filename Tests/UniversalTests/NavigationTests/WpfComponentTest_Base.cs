using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using HTML_WPF.Component;
using Tests.Infra.HTMLEngineTesterHelper.HtmlContext;
using Tests.Infra.HTMLEngineTesterHelper.Window;
using Tests.Infra.IntegratedContextTesterHelper.Window;
using Neutronium.Core;
using Neutronium.Core.Infra;

namespace IntegratedTest.Tests.WPF
{
    public abstract class WpfComponentTest_Base<T>  where T : HTMLControlBase
    {
        protected readonly WindowTestContext _WindowTestEnvironment;
        protected readonly IWebSessionLogger _Logger;

        protected WpfComponentTest_Base(IWindowContextProvider windowTestEnvironment) 
        {
            _Logger = new BasicLogger();
            _WindowTestEnvironment = windowTestEnvironment.WindowTestContext;
        }

        protected string GetPath(TestContext context) 
        {
             return _WindowTestEnvironment.HtmlProvider.GetHtlmPath(context);
        }

        protected string GetRelativePath(TestContext context)
        {
            return _WindowTestEnvironment.HtmlProvider.GetRelativeHtlmPath(context);
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

        private WindowTest InitTest(HTMLControlBase_Handler<T> handler, bool iDebug = false, bool iManageLifeCycle = true)
        {
            AssemblyHelper.SetEntryAssembly();
            Func<T> iWebControlFac = () => 
            {
                var element = GetNewHTMLControlBase(iDebug);
                element.WebSessionLogger = _Logger;
                return handler.Handler = element;
            };

            return BuildWindow(iWebControlFac, iManageLifeCycle);
        }

        internal void Test(Action<T> test, bool iDebug = false, bool iManageLifeCycle = true) 
        {
            var handler = new HTMLControlBase_Handler<T>();
            using (var wcontext = InitTest(handler, iDebug, iManageLifeCycle))
            {
                wcontext.RunOnUIThread( () => test(handler.Handler)).Wait();
            }
        }

        internal async Task Test(Func<T, WindowTest, Task> test, bool iDebug = false, bool iManageLifeCycle = true) 
        {
            var handler = new HTMLControlBase_Handler<T>();
            using (var wcontext = InitTest(handler, iDebug, iManageLifeCycle)) 
            {
                await wcontext.RunOnUIThread(async () => await test(handler.Handler, wcontext));
            }
        }
    }
}
