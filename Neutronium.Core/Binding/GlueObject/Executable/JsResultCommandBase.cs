using System;
using System.Threading.Tasks;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.Converter;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.Core.Binding.GlueObject.Executable
{
    internal abstract class JsResultCommandBase<TResult, TJsContext> : GlueBase
    {
        private readonly HtmlViewContext _HtmlViewContext;
        private readonly ICSharpUnrootedObjectManager _CSharpUnrootedObjectManager;

        public virtual IJavascriptObject CachableJsValue => JsValue;
        public JsCsGlueType Type => JsCsGlueType.ResultCommand;
        protected IWebView WebView => _HtmlViewContext.WebView;
        private IDispatcher UiDispatcher => _HtmlViewContext.UiDispatcher;
        public uint JsId { get; private set; }
        public void SetJsId(uint jsId) => JsId = jsId;

        protected IWebSessionLogger Logger => _HtmlViewContext.Logger;
        protected readonly IJavascriptToGlueMapper JavascriptToGlueMapper;

        protected JsResultCommandBase(HtmlViewContext context, ICSharpUnrootedObjectManager manager, IJavascriptToGlueMapper converter)
        {
            _HtmlViewContext = context;
            JavascriptToGlueMapper = converter;
            _CSharpUnrootedObjectManager = manager;
        }

        public virtual void SetJsValue(IJavascriptObject value, IJavascriptSessionCache sessionCache)
        {
            SetJsValue(value);
            sessionCache.Cache((IJsCsGlue)this);
        }

        public void UpdateJsObject(IJavascriptObject javascriptObject)
        {
            var executable = this as IExecutableGlue;
            javascriptObject.Bind("Execute", WebView, executable.Execute);
        }

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder)
        {
            builder.RequestExecutableCreation();
        }

        public void VisitDescendants(Func<IJsCsGlue, bool> visit)
        {
            visit((IJsCsGlue)this);
        }

        public void VisitChildren(Action<IJsCsGlue> visit) { }

        protected abstract Task<TResult> ExecuteOnUiThread(TJsContext context);
        protected abstract MayBe<TJsContext> GetArgumentValueOnJsContext(IJavascriptObject[] e);
        protected abstract IJavascriptObject GetPromise(IJavascriptObject[] e);

        public async void Execute(IJavascriptObject[] arguments)
        {
            var promise = GetPromise(arguments);
            var argument = GetArgumentValueOnJsContext(arguments);
            if (!argument.Success)
                return;

            try
            {
                await await UiDispatcher.EvaluateAsync(async () =>
                {
                    var res = await ExecuteOnUiThread(argument.Value);
                    _CSharpUnrootedObjectManager.RegisterInSession(res, bridge => SetResult(promise, bridge));
                });
            }
            catch (Exception exception)
            {
                SetError(promise, exception).DoNotWait();
            }
        }

        private async Task SetError(IJavascriptObject promise, Exception exception)
        {
            if (promise == null)
                return;

            await WebView.RunAsync(async () =>
            {
                var errorMessage = exception?.Message ?? "Faulted";
                await promise.InvokeAsync("reject", WebView, WebView.Factory.CreateString(errorMessage));
            });
        }

        private async void SetResult(IJavascriptObject promise, IJsCsGlue bridgeValue)
        {
            if (promise == null)
                return;

            await WebView.RunAsync(async () =>
            {
                await promise.InvokeAsync("fullfill", WebView, bridgeValue.GetJsSessionValue());
            });
        }

        protected override void ComputeString(IDescriptionBuilder context)
        {
            context.AppendCommandDescription(true);
        }

        public void ApplyOnListenable(IObjectChangesListener listener)
        {
        }
    }
}
