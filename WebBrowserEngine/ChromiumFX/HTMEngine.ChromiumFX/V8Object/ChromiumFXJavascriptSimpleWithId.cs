using Chromium.Remote;

namespace Neutronium.WebBrowserEngine.ChromiumFx.V8Object
{
    internal sealed class ChromiumFXJavascriptSimpleWithId : ChromiumFXJavascriptSimpleBase, ICfxJavascriptObject
    {
        private readonly uint _Id;

        internal ChromiumFXJavascriptSimpleWithId(CfrV8Value cfrV8Value, uint id) : base(cfrV8Value)
        {
            _Id = id;
        }

        public uint GetID() => _Id;
    }
}
