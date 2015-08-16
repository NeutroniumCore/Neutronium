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

using Xilium.CefGlue.WPF;
using CefGlue.Window;
using MVVM.CEFGlue.Binding.HTMLBinding.V8JavascriptObject;

namespace MVVM.CEFGlue
{
    public class HTML_Binding : IDisposable, IHTMLBinding
    {
        private BidirectionalMapper _BirectionalMapper;
        private IWebView _CefV8Context;

        private HTML_Binding(IWebView iContext, BidirectionalMapper iConvertToJSO)
        {
            _CefV8Context = iContext;
            _BirectionalMapper = iConvertToJSO;
        }

        public IJavascriptObject JSRootObject
        {
            get { return _BirectionalMapper.JSValueRoot.GetJSSessionValue(); }
        }

        public IWebView Context { get { return _CefV8Context; } }

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

        internal static async Task<IHTMLBinding> Bind(ICefGlueWindow view, object iViewModel, JavascriptBindingMode iMode, object additional = null)
        {      
            var context = view.MainFrame.GetMainContext();
            var mapper = await context.EvaluateAsync(() => new BidirectionalMapper(iViewModel, context, iMode, additional));
            await mapper.Init();
            return new HTML_Binding(context, mapper);
        }
    }
}
