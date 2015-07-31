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
        private CefV8CompleteContext _CefV8Context;

        private HTML_Binding(CefV8CompleteContext iContext, BidirectionalMapper iConvertToJSO)
        {
            _CefV8Context = iContext;
            _BirectionalMapper = iConvertToJSO;
        }

        public CefV8Value JSRootObject
        {
            get { return _BirectionalMapper.JSValueRoot.GetJSSessionValue(); }
        }

        public CefV8CompleteContext Context { get { return _CefV8Context; } }

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
        }

        internal static Task<IHTMLBinding> Bind(ICefGlueWindow view, object iViewModel, object additional, JavascriptBindingMode iMode)
        {
            TaskCompletionSource<IHTMLBinding> tcs = new TaskCompletionSource<IHTMLBinding>();

            view.ExecuteWhenReady(() =>
                    {
                        var context = view.MainFrame.GetMainContext();
                        try 
                        { 
                            var mapper = new BidirectionalMapper(iViewModel, context, iMode, additional);
                            mapper.Init().ContinueWith(t => 
                            {
                                if (t.IsFaulted)
                                {
                                    tcs.SetException(t.Exception);
                                }
                                else tcs.SetResult(new HTML_Binding(context, mapper));
                            });
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
            return Bind(view, iViewModel, null, iMode);
        }

    }
}
