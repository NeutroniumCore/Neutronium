using System;
using Tests.Infra.HTMLEngineTesterHelper.Window;

namespace Tests.Infra.IntegratedContextTesterHelper.Window 
{
    public abstract class WindowContextProvider: IWindowContextProvider, IDisposable 
    {
        private WindowTestContext _WindowTestContext = null;
        private readonly WpfThread _WpfThread;
        public abstract WindowTestContext GetWindowTestContext();

        protected WindowContextProvider() 
        {
            _WpfThread = WpfThread.GetWpfThread();
            _WpfThread.AddRef();
        }

        public WindowTestContext WindowTestContext 
        {
            get 
            {
                if (_WindowTestContext != null)
                    return _WindowTestContext;

                return _WindowTestContext = GetWindowTestContext();
            }
        }

        public void Dispose() 
        {
            _WpfThread.Release();
        }
    }
}
