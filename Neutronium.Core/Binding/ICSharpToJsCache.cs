using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Binding
{
    public interface ICSharpToJsCache
    {
        IJsCsGlue GetCached(object key);

        void CacheFromCSharpValue(object key, IJsCsGlue value);

    }
}
