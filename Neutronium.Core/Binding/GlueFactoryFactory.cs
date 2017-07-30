using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Binding
{
    internal static class GlueFactoryFactory
    {
        internal static IGlueFactory GetFactory(HTMLViewContext context, IJavascriptToCSharpConverter converter)
        {
            return new GlueFactory(context, converter);
        }
    }
}
