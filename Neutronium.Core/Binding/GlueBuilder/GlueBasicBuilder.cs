using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.GlueObject.Factory;

namespace Neutronium.Core.Binding.GlueBuilder 
{
    internal sealed class GlueBasicBuilder : ICsToGlueConverter
    {
        private readonly IGlueFactory _GlueFactory;

        public GlueBasicBuilder(IGlueFactory factory) 
        {
            _GlueFactory = factory;
        }

        public IJsCsGlue Convert(object @object) => _GlueFactory.BuildBasic(@object);
    }
}
