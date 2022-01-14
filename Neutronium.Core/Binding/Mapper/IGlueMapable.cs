using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Updater;

namespace Neutronium.Core.Binding.Mapper
{
    public interface IGlueMapable
    {
        object Source { get; }
        IJsCsGlue Map(IJsUpdateHelper helper);
    }
}
