using System.Threading.Tasks;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace MVVM.HTML.Core
{
    public class StringBinding :  IHTMLBinding
    {
        private IJavascriptSessionInjector _JavascriptSessionInjector;
        private readonly IJavascriptObject _Root;
        private readonly HTMLViewContext _Context;

        internal StringBinding(HTMLViewContext context, IJavascriptObject root, IJavascriptSessionInjector iKnockoutSessionInjector)
        {
            _JavascriptSessionInjector = iKnockoutSessionInjector;
            _Context = context;
            _Root = root;
        }

        public void Dispose()
        {
            Context.RunAsync(() =>
            {
                if (_JavascriptSessionInjector != null)
                {
                    _JavascriptSessionInjector.Dispose();
                    _JavascriptSessionInjector = null;
                }
            });
        }

        public JavascriptBindingMode Mode { get { return JavascriptBindingMode.OneTime; } }

        public IJavascriptObject JSRootObject
        {
            get { return _Root; }
        }

        public object Root
        {
            get { return null; }
        }

        public IJavascriptSessionInjector JavascriptUIFramework
        {
            get { return _Context.JavascriptSessionInjector; }
        }

        public static async Task<IHTMLBinding> Bind(HTMLViewEngine engine, string iViewModel)
        {
            var context = engine.GetMainContext();
            var mainView = context.WebView;

            var root = await mainView.EvaluateAsync(() =>
                {
                    var json = mainView.GetGlobal().GetValue("JSON");
                    return json.Invoke("parse", mainView, mainView.Factory.CreateString(iViewModel));
                });

            var injector = context.CreateInjector(null);
            var mappedroot = injector.Inject(root, null);
            await injector.RegisterMainViewModel(mappedroot);

            return new StringBinding(context, mappedroot, injector);
        }

        public IWebView Context
        {
            get { return _Context.WebView; }
        }
    }
}
