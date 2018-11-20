using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Neutronium.Core;
using Neutronium.Core.Binding;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Tests.Infra.JavascriptFrameworkTesterHelper;
using Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext;
using Tests.Infra.WebBrowserEngineTesterHelper.Windowless;
using Xunit.Abstractions;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Tests.Infra.IntegratedContextTesterHelper.Windowless
{
    public abstract class IntegratedTestBase : TestBase
    {
        private IJavascriptFrameworkExtractor _JavascriptFrameworkExtractor;
        private readonly IWindowLessHTMLEngineProvider _WindowLessHTMLEngineProvider;
        private IUiDispatcher _UIDispatcher;

        protected bool SupportDynamicBinding => _JavascriptFrameworkExtractor.SupportDynamicBinding;

        protected IntegratedTestBase(IWindowLessHTMLEngineProvider testEnvironment, ITestOutputHelper output): base (testEnvironment.WindowBuilder, output) 
        {
            _WindowLessHTMLEngineProvider = testEnvironment;
        }

        protected override IWindowlessHTMLEngine Tester(TestContext context = TestContext.Index) 
        {
            var tester = base.Tester(context);
            var frameworkHelper = _WindowLessHTMLEngineProvider.FrameworkTestContext;
            _ViewEngine = new HtmlViewEngine(tester.HTMLWindowProvider, frameworkHelper.FrameworkManager, _Logger);
            _UIDispatcher = tester.HTMLWindowProvider.UiDispatcher;
            _JavascriptFrameworkExtractor = frameworkHelper.JavascriptFrameworkExtractorBuilder(_WebView);
            return tester;
        }

        private async Task RunAsync(TestContextBase test, Func<IHtmlBinding, Task> Run, string memberName)
        {
            _Logger.Info($"Starting {memberName}");
            using (Tester(test.Path))
            {
                _Logger.Info("Begin Binding");
                using (var mb = await EvaluateSafeUI(() => test.Bind(_ViewEngine)))
                {
                    _Logger.Info("End Binding");
                    _Logger.Info("Begin Run");
                    await Run(mb);
                    _Logger.Info("Ending Run");
                }
                _Logger.Info($"Ending {memberName}");
            }
        }

        protected Task RunAsync(TestInContext test, [CallerMemberName] string memberName = "")
        {
            return RunAsync(test, mb => RunInContext(() => test.Test(mb)), memberName); 
        }

        protected Task RunAsync(TestInContextAsync test, [CallerMemberName] string memberName = "") 
        {
            return RunAsync(test, mb => RunInContext(async () => await test.Test(mb)), memberName);        
        }

        protected void SetAttribute(IJavascriptObject father, string attibutename, IJavascriptObject value)
        {
            _JavascriptFrameworkExtractor.SetAttribute(father, attibutename, value);
        }

        protected void AddAttribute(IJavascriptObject father, string attibutename, IJavascriptObject value)
        {
            _JavascriptFrameworkExtractor.AddAttribute(father, attibutename, value);
        }

        protected IJavascriptObject GetAttribute(IJavascriptObject value, string attibutename)
        {
            return _JavascriptFrameworkExtractor.GetAttribute(value, attibutename);
        }

        protected IJavascriptObject GetCollectionAttribute(IJavascriptObject value, string attibutename)
        {
            return _JavascriptFrameworkExtractor.GetCollectionAttribute(value, attibutename);
        }

        protected string GetStringAttribute(IJavascriptObject value, string attibutename)
        {
            return _JavascriptFrameworkExtractor.GetStringAttribute(value, attibutename);
        }

        protected int GetIntAttribute(IJavascriptObject value, string attibutename)
        {
            return _JavascriptFrameworkExtractor.GetIntAttribute(value, attibutename);
        }

        protected double GetDoubleAttribute(IJavascriptObject value, string attibutename)
        {
            return _JavascriptFrameworkExtractor.GetDoubleAttribute(value, attibutename);
        }

        protected bool GetBoolAttribute(IJavascriptObject value, string attibutename)
        {
            return _JavascriptFrameworkExtractor.GetBoolAttribute(value, attibutename);
        }

        protected IJavascriptObject GetRootViewModel()
        {
            return _JavascriptFrameworkExtractor.GetRootViewModel();
        }

        protected void DoSafeUI(Action doact)
        {
            _UIDispatcher.Run(doact);
        }

        protected async Task DoSafeAsyncUI(Action doact)
        {
            await _UIDispatcher.RunAsync(doact);
        }

        protected T EvaluateSafeUI<T>(Func<T> doact)
        {
            return _UIDispatcher.Evaluate(doact);
        }
    }
}
