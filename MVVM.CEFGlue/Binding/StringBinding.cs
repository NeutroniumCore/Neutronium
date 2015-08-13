using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xilium.CefGlue;

using MVVM.CEFGlue.HTMLBinding;
using MVVM.CEFGlue.CefGlueHelper;
using Xilium.CefGlue.WPF;
using CefGlue.Window;
using MVVM.CEFGlue.Binding.HTMLBinding.V8JavascriptObject;



namespace MVVM.CEFGlue
{
    public class StringBinding : IDisposable, IHTMLBinding
    {
        private JavascriptSessionInjector _JavascriptSessionInjector;
        private CefV8Value _Root;
        private IWebView _Context;

        internal StringBinding(IWebView context, CefV8Value root, JavascriptSessionInjector iJavascriptSessionInjector)
        {
            _JavascriptSessionInjector = iJavascriptSessionInjector;
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

        public CefV8Value JSRootObject
        {
            get { return _Root; }
        }

        public object Root
        {
            get { return null; }
        }

        public static async Task<IHTMLBinding> Bind(ICefGlueWindow view, string iViewModel)
        {
            var context = view.MainFrame.GetMainContext();

            var root = await context.EvaluateAsync(() =>
                {
                    var json = context.GetGlobal().GetValue("JSON");
                    return json.Invoke("parse", context, CefV8Value.CreateString(iViewModel));
                });

            var injector = new JavascriptSessionInjector(context, new GlobalBuilder(context, "MVVMGlue"), null);
            var mappedroot = injector.Map(root, null);
            await injector.RegisterInSession(mappedroot);

            return new StringBinding(context, mappedroot, injector);
        }

        public IWebView Context
        {
            get { return _Context; }
        }
    }
}
