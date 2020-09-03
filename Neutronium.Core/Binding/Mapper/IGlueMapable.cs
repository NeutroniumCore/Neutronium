using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Updater;

namespace Neutronium.Core.Binding.Mapper
{
    internal interface IGlueMapable
    {
        object Source { get; }
        IJsCsGlue Map(IJsUpdateHelper helper);
    }
}
