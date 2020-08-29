using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Updater;

namespace Neutronium.Core.Binding.Mapper
{
    internal interface IGlueConvertible
    {
        object Source { get; }
        IJsCsGlue Convert(IJsUpdateHelper helper);
    }
}
