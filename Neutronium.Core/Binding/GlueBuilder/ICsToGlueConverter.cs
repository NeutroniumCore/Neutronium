using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Binding.GlueBuilder 
{
    public interface ICsToGlueConverter 
    {
        IJsCsGlue Convert(object @object);
    }
}
