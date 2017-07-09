using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Binding.Builder
{
    public interface IJavascriptObjectBuilderStrategy
    {
        void UpdateJavascriptValue(IJSCSGlue root);
    }
}
