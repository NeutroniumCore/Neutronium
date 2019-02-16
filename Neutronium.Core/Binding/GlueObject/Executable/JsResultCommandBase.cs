using System;
using System.Threading.Tasks;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.Core.Binding.GlueObject.Executable
{
    internal abstract class JsResultCommandBase<TResult, TJsContext> : GlueBase
    {
        private readonly HtmlViewContext _HtmlViewContext;

        public virtual IJavascriptObject CachableJsValue => JsValue;
        public JsCsGlueType Type => JsCsGlueType.ResultCommand;
        protected IWebView WebView => _HtmlViewContext.WebView;
        private IDispatcher UiDispatcher => _HtmlViewContext.UiDispatcher;

        private uint _JsId;
        public uint JsId => _JsId;
        public void SetJsId(uint jsId) => _JsId = jsId;

        protected IWebSessionLogger Logger => _HtmlViewContext.Logger;
        protected readonly IJavascriptToCSharpConverter JavascriptToCSharpConverter;


        protected JsResultCommandBase(HtmlViewContext context, IJavascriptToCSharpConverter converter)
        {
            _HtmlViewContext = context;
            JavascriptToCSharpConverter = converter;
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

        protected abstract Task<MayBe<TResult>> ExecuteOnUiThread(TJsContext context);
        protected abstract MayBe<TJsContext> ExecuteOnJsContextThread(IJavascriptObject[] e);
        protected abstract IJavascriptObject GetPromise(IJavascriptObject[] e);

        public async void Execute(IJavascriptObject[] arguments)
        {
            var promise = GetPromise(arguments);
            var context = ExecuteOnJsContextThread(arguments);
            if (!context.Success)
                return;

            try
            {
                await await UiDispatcher.EvaluateAsync(async () =>
                {
                    var res = await ExecuteOnUiThread(context.Value);
                    if (!res.Success)
                        return;

                    JavascriptToCSharpConverter.RegisterInSession(res.Value, bridge => SetResult(promise, bridge));
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
