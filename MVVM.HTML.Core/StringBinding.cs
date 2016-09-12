using System.Threading.Tasks;
using Neutronium.Core.Binding;
using Neutronium.Core.JavascriptEngine.JavascriptObject;
using Neutronium.Core.JavascriptFramework;

namespace Neutronium.Core
{
    public class StringBinding :  IHTMLBinding
    {
        private readonly IJavascriptObject _Root;
        private readonly HTMLViewContext _Context;

        public JavascriptBindingMode Mode => JavascriptBindingMode.OneTime;
        public IJavascriptObject JSRootObject => _Root;
        public object Root => null;
        public IJavascriptSessionInjector JavascriptUIFramework => _Context.JavascriptSessionInjector;
        public IWebView Context => _Context.WebView;

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
            //TODO improve context management or remove this functionality
            var mappedroot = await mainView.EvaluateAsync( () => injector.Inject(root, null));
            await injector.RegisterMainViewModel(mappedroot);

            return new StringBinding(context, mappedroot);
        }
    }
}
