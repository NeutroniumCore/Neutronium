using System.Threading.Tasks;
using Neutronium.Core.Binding;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core
{
    public class StringBinding :  IHtmlBinding
    {
        private readonly IJavascriptObject _Root;
        private readonly HtmlViewContext _Context;

        public JavascriptBindingMode Mode => JavascriptBindingMode.OneTime;
        public IJavascriptObject JsRootObject => _Root;
        public object Root => null;
        public IJavascriptSessionInjector JavascriptUiFramework => _Context.JavascriptSessionInjector;
        public IWebView Context => _Context.WebView;
        public IJsCsGlue JsBrideRootObject => null;


        internal StringBinding(HtmlViewContext context, IJavascriptObject root)
        {
            _Context = context;
            _Root = root;
        }

        public void Dispose()
        {
            Context.RunAsync(() =>
            {
                _Context.Dispose();
            });
        }

        public static async Task<IHtmlBinding> Bind(HtmlViewEngine engine, string viewModel)
        {
            var context = engine.GetMainContext();
            var mainView = context.WebView;

            var root = await mainView.EvaluateAsync(() =>
                {
                    var json = mainView.GetGlobal().GetValue("JSON");
                    context.InitOnJsContext(null, false);
                    return json.Invoke("parse", mainView, mainView.Factory.CreateString(viewModel));
                });

            var injector = context.JavascriptSessionInjector;
            //TODO improve context management or remove this functionality

            var mappedroot= await await mainView.EvaluateAsync(async () => {
                var rootMapped = await mainView.EvaluateAsync(() => injector.Inject(root, null));
                await injector.RegisterMainViewModel(rootMapped);
                return rootMapped;
            });
           
            return new StringBinding(context, mappedroot);
        }
    }
}
