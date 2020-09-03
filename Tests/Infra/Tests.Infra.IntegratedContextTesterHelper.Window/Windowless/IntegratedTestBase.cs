using Neutronium.Core.Binding;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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

        protected async Task SetAttributeAsync(IJavascriptObject father, string attributeName, IJavascriptObject value)
        {
            await _JavascriptFrameworkExtractor.SetAttributeAsync(father, attributeName, value);

            await WaitAnotherWebContextCycle();
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

        protected async Task<IJavascriptObject> GetCollectionAttributeAsync(IJavascriptObject value, string attributeName)
        {
            await WaitAnotherWebContextCycle();

            return await _JavascriptFrameworkExtractor.GetCollectionAttributeAsync(value, attributeName);
        }

        protected string GetStringAttribute(IJavascriptObject value, string attributeName)
        {
            return _JavascriptFrameworkExtractor.GetAttribute(value, attributeName).GetStringValue();
        }

        protected async Task<IJavascriptObject> GetAttributeAsync(IJavascriptObject value, string attributeName)
        {
            await WaitAnotherWebContextCycle();

            return await _JavascriptFrameworkExtractor.GetAttributeAsync(value, attributeName);
        }

        protected async Task<string> GetStringAttributeAsync(IJavascriptObject value, string attributeName)
        {
            var jsObject = await GetAttributeAsync(value, attributeName);
            return jsObject.GetStringValue();
        }

        protected async Task<int> GetIntAttributeAsync(IJavascriptObject value, string attributeName)
        {
            var jsObject = await GetAttributeAsync(value, attributeName);
            return jsObject.GetIntValue();
        }

        protected async Task<double> GetDoubleAttributeAsync(IJavascriptObject value, string attributeName)
        {
            var jsObject = await GetAttributeAsync(value, attributeName);
            return jsObject.GetDoubleValue();
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
            await WaitAnotherUiCycle();

            await _UIDispatcher.RunAsync(act);

            await WaitAnotherUiCycle();
        }

        protected async Task DoSafeAsyncUIFullCycle(Action act)
        {
            await _UIDispatcher.RunAsync(act);

            await WaitAnotherUiCycle();

            await WaitAnotherWebContextCycle();
        }

        protected async Task DoSafeAsync(Action action)
        {
            await _WebView.RunAsync(action);

            await WaitAnotherWebContextCycle();
        }

        protected async Task<T> EvaluateAsync<T>(Func<T> unsafeGet)
        {
            var result = await _WebView.EvaluateAsync(unsafeGet);
            await WaitAnotherWebContextCycle();
            return result;
        }

        protected async Task WaitOneCompleteCycle()
        {
            await WaitAnotherWebContextCycle();
            await WaitAnotherUiCycle();
        }

        protected Task WaitAnotherUiCycle()
        {
            var completion = new TaskCompletionSource<bool>();
            _UIDispatcher.DispatchWithBindingPriority(() => completion.SetResult(true));
            return completion.Task;
        }

        protected async Task WaitAnotherWebContextCycle()
        {
            await Task.Delay(1);
        }

        protected void Call(IJavascriptObject value, string functionName, params IJavascriptObject[] parameter)
        {
            _WebView.Run(() => value.Invoke(functionName, _WebView, parameter));
        }

        protected async Task CallAsync(IJavascriptObject value, string functionName, params IJavascriptObject[] parameter)
        {
            await _WebView.RunAsync(() => value.Invoke(functionName, _WebView, parameter));
            await WaitAnotherWebContextCycle();
        }

        private T EvaluateSafeUI<T>(Func<T> compute)
        {
            return _UIDispatcher.Evaluate(compute);
        }

        protected Task<T> EvaluateSafeUIAsync<T>(Func<T> compute)
        {
            return _UIDispatcher.EvaluateAsync(compute);
        }

        protected IJavascriptObject CallWithRes(IJavascriptObject value, string functionName, params IJavascriptObject[] parameter)
        {
            return _WebView.Evaluate(() => value.Invoke(functionName, _WebView, parameter));
        }

        protected async Task<IJavascriptObject> CallWithResAsync(IJavascriptObject value, string functionName, params IJavascriptObject[] parameter)
        {
            await WaitAnotherWebContextCycle();

            return await _WebView.EvaluateAsync(() => value.Invoke(functionName, _WebView, parameter));
        }
    }
}
