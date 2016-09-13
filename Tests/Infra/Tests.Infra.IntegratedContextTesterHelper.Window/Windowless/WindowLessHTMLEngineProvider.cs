using System;
using Tests.Infra.JavascriptFrameworkTesterHelper;
using Tests.Infra.WebBrowserEngineTesterHelper.Context;

namespace Tests.Infra.IntegratedContextTesterHelper.Windowless 
{
    public abstract class WindowLessHTMLEngineProvider : IWindowLessHTMLEngineProvider, IDisposable 
    {
        private IBasicWindowLessHTMLEngineProvider _BasicWindowLessHTMLEngineProvider;
        public abstract FrameworkTestContext FrameworkTestContext { get; }

        public IBasicWindowLessHTMLEngineProvider WindowBuilder 
        {
            get 
            {
                if (_BasicWindowLessHTMLEngineProvider != null)
                    return _BasicWindowLessHTMLEngineProvider;

                return _BasicWindowLessHTMLEngineProvider = GetBasicWindowLessHTMLEngineProvider();
            }
        }

        protected abstract IBasicWindowLessHTMLEngineProvider GetBasicWindowLessHTMLEngineProvider();

        public void Dispose() 
        {
            _BasicWindowLessHTMLEngineProvider.Dispose();
        }
    }
}
