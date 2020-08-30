using Neutronium.Core.Binding;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Threading;
using Tests.Infra.JavascriptFrameworkTesterHelper;
using Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext;
using Tests.Infra.WebBrowserEngineTesterHelper.Windowless;
using Xunit.Abstractions;

namespace Tests.Infra.IntegratedContextTesterHelper.Windowless
{
    public abstract class IntegratedTestBase : TestBase
    {
        private IJavascriptFrameworkExtractor _JavascriptFrameworkExtractor;
        private readonly IWindowLessHTMLEngineProvider _WindowLessHTMLEngineProvider;
        private IUiDispatcher _UIDispatcher;

        protected bool SupportDynamicBinding => _JavascriptFrameworkExtractor.SupportDynamicBinding;

        protected IntegratedTestBase(IWindowLessHTMLEngineProvider testEnvironment, ITestOutputHelper output) : base(testEnvironment.WindowBuilder, output)
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

        private async Task RunAsync<TContext>(TestContextBase<TContext> test, Func<TContext, Task> run, string memberName) where TContext:IDisposable
        {
            _Logger.Info($"Starting {memberName}");
            using (Tester(test.Path))
            {
                _Logger.Info("Begin Binding");
                var mb = await EvaluateSafeUI(() => test.Bind(_ViewEngine));
                _Logger.Info("End Binding");
                _Logger.Info("Begin Run");
                await run(mb);
                _Logger.Info("Ending Run");
                await DoSafeAsyncUI(() => mb.Dispose());
                _Logger.Info($"Ending {memberName}");
            }
        }

        protected Task RunAsync<TContext>(TestInContext<TContext> test, [CallerMemberName] string memberName = "") where TContext : IDisposable
        {
            return RunAsync(test, mb => RunInContext(() => test.Test(mb)), memberName);
        }

        protected Task RunAsync<TContext>(TestInContextAsync<TContext> test, [CallerMemberName] string memberName = "") where TContext : IDisposable
        {
            return RunAsync(test, mb => RunInContext(async () => await test.Test(mb)), memberName);
        }

        protected void SetAttribute(IJavascriptObject father, string attributeName, IJavascriptObject value)
        {
            _JavascriptFrameworkExtractor.SetAttribute(father, attributeName, value);
        }

        protected void AddAttribute(IJavascriptObject father, string attributeName, IJavascriptObject value)
        {
            _JavascriptFrameworkExtractor.AddAttribute(father, attributeName, value);
        }

        protected IJavascriptObject GetAttribute(IJavascriptObject value, string attributeName)
        {
            return _JavascriptFrameworkExtractor.GetAttribute(value, attributeName);
        }

        protected IJavascriptObject GetCollectionAttribute(IJavascriptObject value, string attributeName)
        {
            return _JavascriptFrameworkExtractor.GetCollectionAttribute(value, attributeName);
        }

        protected string GetStringAttribute(IJavascriptObject value, string attributeName)
        {
            return _JavascriptFrameworkExtractor.GetStringAttribute(value, attributeName);
        }

        protected int GetIntAttribute(IJavascriptObject value, string attributeName)
        {
            return _JavascriptFrameworkExtractor.GetIntAttribute(value, attributeName);
        }

        protected double GetDoubleAttribute(IJavascriptObject value, string attributeName)
        {
            return _JavascriptFrameworkExtractor.GetDoubleAttribute(value, attributeName);
        }

        protected bool GetBoolAttribute(IJavascriptObject value, string attributeName)
        {
            return _JavascriptFrameworkExtractor.GetBoolAttribute(value, attributeName);
        }

        protected IJavascriptObject GetRootViewModel()
        {
            return _JavascriptFrameworkExtractor.GetRootViewModel();
        }

        protected async Task DoSafeAsyncUI(Action act)
        {
            await _UIDispatcher.RunAsync(act);

            await WaitAnotherUiCycle();
        }

        protected async Task WaitAnotherUiCycle()
        {
            await _UIDispatcher.RunAsync(() => { });
        }

        private T EvaluateSafeUI<T>(Func<T> compute)
        {
            return _UIDispatcher.Evaluate(compute);
        }

        protected Task<T> EvaluateSafeUIAsync<T>(Func<T> compute)
        {
            return _UIDispatcher.EvaluateAsync(compute);
        }
    }
}
