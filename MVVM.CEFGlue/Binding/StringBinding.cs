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



namespace MVVM.CEFGlue
{
    public class StringBinding : IDisposable, IHTMLBinding
    {
        private JavascriptSessionInjector _JavascriptSessionInjector;
        private CefV8Value _Root;
        private CefV8CompleteContext _Context;

        internal StringBinding(CefV8CompleteContext context, CefV8Value root, JavascriptSessionInjector iJavascriptSessionInjector)
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
            var v8context = context.Context;

            var root = await context.EvaluateAsync(() =>
                {
                    var json = v8context.GetGlobal().GetValue("JSON");
                    return json.Invoke("parse", context, CefV8Value.CreateString(iViewModel));
                });

            var injector = new JavascriptSessionInjector(context, new GlobalBuilder(context, "MVVMGlue"), null);
            var mappedroot = injector.Map(root, null);
            await injector.RegisterInSession(mappedroot);

            return new StringBinding(context, mappedroot, injector);
        }

        public CefV8CompleteContext Context
        {
            get { return _Context; }
        }
    }
}
