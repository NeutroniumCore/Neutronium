using Chromium.Remote;

namespace Neutronium.WebBrowserEngine.ChromiumFx.V8Object
{
    internal sealed class ChromiumFXJavascriptObjectWithId : ChromiumFXJavascriptObjectBase, ICfxJavascriptObject
    {
        private readonly uint _Id;

        internal ChromiumFXJavascriptObjectWithId(CfrV8Value cfrV8Value, uint id) :base(cfrV8Value)
        {
            _Id = id;
        }

        public uint GetID() => _Id;
    }
}
