using Chromium.Remote;

namespace Neutronium.WebBrowserEngine.ChromiumFx.EngineBinding
{
    internal class ChromiumFxObjectCreationCallBack
    {
        private CfrV8Value[] _Result;

        internal CfrV8Handler Handler { get; }

        internal ChromiumFxObjectCreationCallBack()
        {
            Handler = new CfrV8Handler();
            Handler.Execute += (_, e) =>
            {
                _Result = e.Arguments;
            };
        }

        public CfrV8Value[] GetLastArguments()
        {
            var result = _Result;
            _Result = result;
            return result;
        }
    }
}
