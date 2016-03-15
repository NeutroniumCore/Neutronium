using System;
using System.Threading.Tasks;
using Chromium;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace HTMEngine.ChromiumFX.EngineBinding
{
    internal class ChromiumFXWebView : IWebView
    {
        private readonly CfxFrame _CfxFrame;

        public ChromiumFXWebView(CfxFrame cfxFrame)
        {
            _CfxFrame = cfxFrame;
        }

        public Task RunAsync(Action act)
        {
            throw new NotImplementedException();
        }

        public void Run(Action act)
        {
            throw new NotImplementedException();
        }

        public Task<T> EvaluateAsync<T>(Func<T> compute)
        {
            throw new NotImplementedException();
        }

        public T Evaluate<T>(Func<T> compute)
        {
            throw new NotImplementedException();
        }

        public IJavascriptObject GetGlobal()
        {
            throw new NotImplementedException();
        }

        public IJavascriptObjectConverter Converter { get; private set; }
        public IJavascriptObjectFactory Factory { get; private set; }
        public bool Eval(string code, out IJavascriptObject res)
        {
            throw new NotImplementedException();
        }

        public void ExecuteJavaScript(string code)
        {
            throw new NotImplementedException();
        }
    }
}
