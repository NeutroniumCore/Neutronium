using Awesomium.Core;
using IntegratedTest;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptUIFramework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MVVM.Awesomium.Tests.Infra
{
    public class AwesomiumWindowlessHTMLEngineFactory : IDisposable
    {
        private bool _Runing = false;
        private readonly IJavascriptUIFrameworkManager _JavascriptUIFrameworkManager;
        private readonly TaskCompletionSource<object> _EndTaskCompletionSource;
        private IWebView _CurrentWebView;

        public AwesomiumWindowlessHTMLEngineFactory(IJavascriptUIFrameworkManager javascriptUIFrameworkManager)
        {
            _JavascriptUIFrameworkManager = javascriptUIFrameworkManager;
            _EndTaskCompletionSource = new TaskCompletionSource<object>();
        }

        private Task Init()
        {
            if (_Runing)
                return TaskHelper.Ended();

            var running = new TaskCompletionSource<object>();
            Task.Run(() =>
            {
                WebCore.Initialize(new WebConfig());
                WebCore.CreateWebSession(WebPreferences.Default);

                WebCore.ShuttingDown += (o, e) =>
                {
                    if (e.Exception != null)
                    {
                        Console.WriteLine("Exception on main thread {0}", e.Exception);
                        e.Cancel = true;
                    }
                };

                WebCore.Started += (o, e) =>
                {
                    AwesomiumWPFWebWindowFactory.WebCoreThread = Thread.CurrentThread;
                    running.TrySetResult(null);
                };

                WebCore.Run();

                _EndTaskCompletionSource.SetResult(null);
            });

            _Runing = true;
            return running.Task;
        }

        public IWindowlessJavascriptEngine CreateWindowlessJavascriptEngine()
        {
            Init().Wait();
            return new AwesomiumWindowlessSharedJavascriptEngine(_JavascriptUIFrameworkManager);
        }

        public void Dispose()
        {
            WebCore.Shutdown();
            _EndTaskCompletionSource.Task.Wait();
        }
    }
}
