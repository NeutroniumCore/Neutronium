using System.Threading.Tasks;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace MVVM.HTML.Core
{
    public class StringBinding :  IHTMLBinding
    {
        private readonly IJavascriptObject _Root;
        private readonly HTMLViewContext _Context;

        internal StringBinding(HTMLViewContext context, IJavascriptObject root)
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

        public JavascriptBindingMode Mode => JavascriptBindingMode.OneTime;

        public IJavascriptObject JSRootObject => _Root;

        public object Root => null;

        public IJavascriptSessionInjector JavascriptUIFramework => _Context.JavascriptSessionInjector;

        public static async Task<IHTMLBinding> Bind(HTMLViewEngine engine, string iViewModel)
        {
            var context = engine.GetMainContext(null);
            var mainView = context.WebView;

            var root = await mainView.EvaluateAsync(() =>
                {
                    var json = mainView.GetGlobal().GetValue("JSON");
                    return json.Invoke("parse", mainView, mainView.Factory.CreateString(iViewModel));
                });

            var injector = context.JavascriptSessionInjector;
            var mappedroot = injector.Inject(root, null);
            await injector.RegisterMainViewModel(mappedroot);

            return new StringBinding(context, mappedroot);
        }

        public IWebView Context => _Context.WebView;
    }
}
