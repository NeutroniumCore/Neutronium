using System;
using System.Threading;
using System.Threading.Tasks;
using Awesomium.Core;
using HTMLEngine.Awesomium;
using Neutronium.WebBrowserEngine.Awesomium;
using Tests.Infra.HTMLEngineTesterHelper.Window;
using Tests.Infra.HTMLEngineTesterHelper.Windowless;

namespace Tests.Awesomium.Infra
{
    public class AwesomiumWindowlessHTMLEngineFactory : IDisposable
    {
        private bool _Runing = false;
        private readonly TaskCompletionSource<object> _EndTaskCompletionSource;
        private readonly WpfThread _WpfThread;

        public AwesomiumWindowlessHTMLEngineFactory()
        {
            _WpfThread = WpfThread.GetWpfThread();
            _WpfThread.AddRef();
            _EndTaskCompletionSource = new TaskCompletionSource<object>();
        }

        private async Task Init()
        {
            if (_Runing)
                return;

            _WpfThread.OnThreadEnded += (o, e) => WebCore.Shutdown();
            var runing = new TaskCompletionSource<object>();
            await _WpfThread.Dispatcher.Invoke(RawInit);
        }

        private Task RawInit() 
        {
            var running = new TaskCompletionSource<object>();
            WebCore.Initialize(new WebConfig());
            WebCore.ShuttingDown += (o, e) => 
            {
                if (e.Exception != null) 
                {
                    Console.WriteLine($"Exception on main thread {e.Exception}");
                    e.Cancel = true;
                }
                _EndTaskCompletionSource.TrySetResult(null);
            };

            WebCore.Started += (o, e) => 
            {
                AwesomiumWPFWebWindowFactory.WebCoreThread = Thread.CurrentThread;
                running.TrySetResult(null);
            };

            WebCore.CreateWebSession(WebPreferences.Default);

            _Runing = true;
            return running.Task;
        }

        public IWindowlessHTMLEngine CreateWindowlessJavascriptEngine()
        {
            Init().Wait();
            return new AwesomiumWindowlessSharedHtmlEngine();
        }

        public void Dispose()
        {
            _WpfThread.Release();
            _EndTaskCompletionSource.Task.Wait();
        }
    }
}
