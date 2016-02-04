using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MVVM.HTML.Core.V8JavascriptObject;
using MVVM.HTML.Core.HTMLBinding;
using MVVM.HTML.Core.Window;
using MVVM.HTML.Core.Binding.Mapping;
using MVVM.HTML.Core.Binding;

namespace MVVM.HTML.Core
{
    public class StringBinding : IDisposable, IHTMLBinding
    {
        private IJavascriptSessionInjector _JavascriptSessionInjector;
        private readonly IJavascriptObject _Root;
        private readonly IWebView _Context;

        internal StringBinding(IWebView context, IJavascriptObject root, IJavascriptSessionInjector iKnockoutSessionInjector)
        {
            _JavascriptSessionInjector = iKnockoutSessionInjector;
            _Context = context;
            _Root = root;
        }

        public void Dispose()
        {
            _Context.RunAsync(() =>
            {
                if (_JavascriptSessionInjector != null)
                {
                    _JavascriptSessionInjector.Dispose();
                    _JavascriptSessionInjector = null;
                }
            });
        }

        public IJavascriptObject JSRootObject
        {
            get { return _Root; }
        }

        public object Root
        {
            get { return null; }
        }

        public static async Task<IHTMLBinding> Bind(HTMLViewEngine engine, string iViewModel)
        {
            var context = engine.HTMLWindowProvider.HTMLWindow.MainFrame;

            var root = await context.EvaluateAsync(() =>
                {
                    var json = context.GetGlobal().GetValue("JSON");
                    return json.Invoke("parse", context, context.Factory.CreateString(iViewModel));
                });

            var injector = engine.SessionInjectorFactory.CreateInjector(context, null);
            var mappedroot = injector.Inject(root, null);
            await injector.RegisterMainViewModel(mappedroot);

            return new StringBinding(context, mappedroot, injector);
        }

        public IWebView Context
        {
            get { return _Context; }
        }
    }
}
