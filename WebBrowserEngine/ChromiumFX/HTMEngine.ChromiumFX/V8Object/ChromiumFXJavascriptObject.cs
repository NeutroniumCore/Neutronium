using Chromium.Remote;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.WebBrowserEngine.ChromiumFx.V8Object
{
    internal sealed class ChromiumFXJavascriptObject : ChromiumFXJavascriptObjectBase, ICfxJavascriptObject
    {
        internal ChromiumFXJavascriptObject(CfrV8Value cfrV8Value) :base(cfrV8Value)
        {
        }

        public uint GetID()
        {
            return (_CfrV8Value.HasValue(NeutroniumConstants.ObjectId)) ? _CfrV8Value.GetValue(NeutroniumConstants.ObjectId).UintValue : 0;
        }
    }
}
