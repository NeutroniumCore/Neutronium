using System;
using System.Windows.Forms;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF 
{
    internal class BrowserWidgetMessageInterceptor : NativeWindow 
     {
        private Func<Message, bool> _ForwardAction;

        internal BrowserWidgetMessageInterceptor(Control browser, IntPtr chromeHostHandle, Func<Message, bool> forwardAction) 
        {
            AssignHandle(chromeHostHandle);
            browser.HandleDestroyed += BrowserHandleDestroyed;
            this._ForwardAction = forwardAction;
        }

        private void BrowserHandleDestroyed(object sender, EventArgs e) 
        {
            ReleaseHandle();
            var browser = (Control) sender;
            browser.HandleDestroyed -= BrowserHandleDestroyed;
            _ForwardAction = null;
        }

        protected override void WndProc(ref Message m) 
        {
            var result = _ForwardAction(m);

            if (!result)
                base.WndProc(ref m);
        }
    }
}