using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Specialized;

using MVVM.CEFGlue.Infra;
using MVVM.CEFGlue.HTMLBinding;
using MVVM.CEFGlue.CefGlueHelper;

using Xilium.CefGlue;
using Xilium.CefGlue.WPF;
using CefGlue.Window;

namespace MVVM.CEFGlue
{
    public class HTML_Binding : IDisposable, IHTMLBinding
    {
        private BidirectionalMapper _BirectionalMapper;
        private Action _CleanUp;
        private CefV8Context _CefV8Context;

        private HTML_Binding(CefV8Context iContext, BidirectionalMapper iConvertToJSO, Action CleanUp = null)
        {
            _CefV8Context = iContext;
            _BirectionalMapper = iConvertToJSO;
            _CleanUp = CleanUp;
        }

        public CefV8Value JSRootObject
        {
            get { return _BirectionalMapper.JSValueRoot.GetJSSessionValue(); }
        }

        public object Root
        {
            get { return _BirectionalMapper.JSValueRoot.CValue; }
        }

        public IJSCSGlue JSBrideRootObject
        {
            get { return _BirectionalMapper.JSValueRoot; }
        }

        public override string ToString()
        {
            return _BirectionalMapper.JSValueRoot.ToString();
        }

        public void Dispose()
        {
            _BirectionalMapper.Dispose();
            if (_CleanUp != null)
            {
                _CefV8Context.RunInContextAsync(() =>
                {
                    _CleanUp();
                    _CleanUp = null;
                });
            }
        }

        //internal static Task<IHTMLBinding> Bind(CefV8Context view, object iViewModel, object additional, JavascriptBindingMode iMode,
        //                                            Action First, Action CleanUp)
        //{
        //    TaskCompletionSource<IHTMLBinding> tcs = new TaskCompletionSource<IHTMLBinding>();

        //    view.ExecuteWhenReady(() =>
        //            {
        //                try 
        //                { 
        //                    if (First != null) First();
        //                    var mapper = new BidirectionalMapper(iViewModel, view, iMode, additional);
        //                    mapper.Init().ContinueWith(_ => tcs.SetResult(new HTML_Binding(view,mapper, CleanUp)));
        //                }
        //                catch (Exception e)
        //                {
        //                    tcs.SetException(e);
        //                }
        //            });

        //    return tcs.Task;
        //}

        internal static Task<IHTMLBinding> Bind(ICefGlueWindow view, object iViewModel, object additional, JavascriptBindingMode iMode,
                                                    Action First, Action CleanUp)
        {
            TaskCompletionSource<IHTMLBinding> tcs = new TaskCompletionSource<IHTMLBinding>();

            view.ExecuteWhenReady(() =>
                    {
                        var context = view.MainFrame.GetMainContext();
                        try 
                        { 
                            if (First != null) First();
                            var mapper = new BidirectionalMapper(iViewModel, context, iMode, additional);
                            mapper.Init().ContinueWith(_ => tcs.SetResult(new HTML_Binding(context, mapper, CleanUp)));
                        }
                        catch (Exception e)
                        {
                            tcs.SetException(e);
                        }
                    });

            return tcs.Task;
        }



        public static Task<IHTMLBinding> Bind(ICefGlueWindow view, object iViewModel, JavascriptBindingMode iMode)
        {
            return Bind(view, iViewModel, null, iMode, null, null);
        }

    }
}
