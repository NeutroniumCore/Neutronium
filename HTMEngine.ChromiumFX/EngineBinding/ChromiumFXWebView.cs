using System;
using System.Threading.Tasks;
using Chromium.Remote;
using HTMEngine.ChromiumFX.Convertion;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace HTMEngine.ChromiumFX.EngineBinding
{
    internal class ChromiumFXWebView : IWebView
    {
        private readonly CfrFrame _CfxFrame;

        public ChromiumFXWebView(CfrFrame cfxFrame)
        {
            _CfxFrame = cfxFrame;
        }

        private CfrV8Context V8Context
        {
            get { return _CfxFrame.V8Context; }
        }

        internal CfrFrame GetRaw()
        {
            return _CfxFrame;
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
            return V8Context.Global.Convert();
        }

        public IJavascriptObjectConverter Converter { get; private set; }
        public IJavascriptObjectFactory Factory { get; private set; }
        public bool Eval(string code, out IJavascriptObject res)
        {
            res = null;
            CfrV8Value v8Res;
            CfrV8Exception exception;
            return (V8Context.Eval(code, out v8Res, out exception);
        }

        public void ExecuteJavaScript(string code)
        {
            _CfxFrame.ExecuteJavaScript(code, String.Empty, 0);
        }
    }
}
