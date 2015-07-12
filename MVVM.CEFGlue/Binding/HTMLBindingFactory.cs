using MVVM.CEFGlue.HTMLBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;
using Xilium.CefGlue.WPF;

namespace MVVM.CEFGlue
{
    public class HTMLBindingFactory : IHTMLBindingFactory
    {
        public HTMLBindingFactory()
        {
            //InjectionTimeOut = 0;
            ManageWebSession = false;
        }

        private Action GetFirst(WpfCefBrowser view)
        {
            return null;
            //throw new NotImplementedException();
            //if (InjectionTimeOut != -1)
            //    return () => view.SynchronousMessageTimeout = InjectionTimeOut;

            //return null;
        }

        private Action GetLast(WpfCefBrowser view)
        {
            //throw new NotImplementedException();

            if (ManageWebSession)
                return () => view.Dispose();

            return null;
        }

        public Task<IHTMLBinding> Bind(WpfCefBrowser view, object iViewModel, JavascriptBindingMode iMode)
        {
            return HTML_Binding.Bind(view, iViewModel, null,iMode, GetFirst(view), GetLast(view));
        }

        public Task<IHTMLBinding> Bind(WpfCefBrowser view, string json)
        {
            return StringBinding.Bind(view, json, GetFirst(view), GetLast(view));
        }

        public Task<IHTMLBinding> Bind(WpfCefBrowser view, object iViewModel, object addinfo, JavascriptBindingMode iMode)
        {
            return HTML_Binding.Bind(view, iViewModel, addinfo, iMode, GetFirst(view), GetLast(view));
        }


        //public int InjectionTimeOut { get;set;}

        public bool ManageWebSession { get; set; }


     
    }
}
