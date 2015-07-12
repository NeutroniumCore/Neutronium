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
        private Action _CleanUp;
        private JavascriptSessionInjector _JavascriptSessionInjector;
        private CefV8Value _Root;
        private CefV8Context _CefV8Context;

        internal StringBinding( CefV8Context context, CefV8Value root, JavascriptSessionInjector iJavascriptSessionInjector, Action CleanUp)
        {
            _JavascriptSessionInjector = iJavascriptSessionInjector;
            _CleanUp = CleanUp;
            _CefV8Context = context;
            _Root = root;
        }

        public void Dispose()
        {
            //WebCore.QueueWork(() =>
            //{
            //    if (_CleanUp != null)
            //    {
            //        _CleanUp();
            //        _CleanUp = null;
            //    }

            //    if (_JavascriptSessionInjector != null)
            //    {
            //        _JavascriptSessionInjector.Dispose();
            //        _JavascriptSessionInjector = null;
            //    }
            //}
            //);

            _CefV8Context.RunInContextAsync(() =>
            {
                if (_CleanUp != null)
                {
                    _CleanUp();
                    _CleanUp = null;
                }

                if (_JavascriptSessionInjector != null)
                {
                    _JavascriptSessionInjector.Dispose();
                    _JavascriptSessionInjector = null;
                }
            } );
        }

        public CefV8Value JSRootObject
        {
            get { return _Root; }
        }

        public object Root
        {
            get { return null; }
        }

        //public static Task<IHTMLBinding> Bind(CefV8Context view, string iViewModel, Action First = null, Action CleanUp = null)
        //{
        //    TaskCompletionSource<IHTMLBinding> tcs = new TaskCompletionSource<IHTMLBinding>();

        //    view.ExecuteWhenReady(() =>
        //    {
        //        if (First != null) First();
        //        CefV8Value json = view.GetGlobal().GetValue("JSON");
        //        var root = json.Invoke("parse", CefV8Value.CreateString(iViewModel));

        //        var injector = new JavascriptSessionInjector(view, new GlobalBuilder(view, "MVVMGlue"), null);
        //        var mappedroot = injector.Map(root, null);
        //        injector.RegisterInSession(mappedroot);

        //        tcs.SetResult(new StringBinding(view, mappedroot, injector, CleanUp));
        //    });

        //    return tcs.Task;
        //}


        public static Task<IHTMLBinding> Bind(ICefGlueWindow view, string iViewModel, Action First = null, Action CleanUp = null)
        {
            TaskCompletionSource<IHTMLBinding> tcs = new TaskCompletionSource<IHTMLBinding>();

            view.ExecuteWhenReady(() =>
            {
                if (First != null) First();

                var context = view.MainFrame.GetMainContext();


                CefV8Value json = context.GetGlobal().GetValue("JSON");
                var root = json.Invoke("parse", context, CefV8Value.CreateString(iViewModel));

                var injector = new JavascriptSessionInjector(context, new GlobalBuilder(context, "MVVMGlue"), null);
                var mappedroot = injector.Map(root, null);
                injector.RegisterInSession(mappedroot);

                tcs.SetResult(new StringBinding(context, mappedroot, injector, CleanUp));
            });

            return tcs.Task;
        }
    }
}
