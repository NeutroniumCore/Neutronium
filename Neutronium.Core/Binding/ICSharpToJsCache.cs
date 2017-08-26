using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Binding
{
    public interface ICSharpToJsCache
    {
        IJSCSGlue GetCached(object key);

        void CacheFromCSharpValue(object key, IJSCSGlue value);

    }
}
