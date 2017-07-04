using Chromium.Remote;

namespace Neutronium.WebBrowserEngine.ChromiumFx.V8Object
{
    internal sealed class ChromiumFXJavascriptObject : ChromiumFXJavascriptObjectBase, ICfxJavascriptObject
    {
        internal ChromiumFXJavascriptObject(CfrV8Value cfrV8Value) :base(cfrV8Value)
        {
        }

        public uint GetID()
        {
            return (_CfrV8Value.HasValue(IdName)) ? _CfrV8Value.GetValue(IdName).UintValue : 0;
        }
    }
}
