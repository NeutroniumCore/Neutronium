using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Updater;

namespace Neutronium.Core.Binding.Mapper
{
    internal struct SolvedGlueMapable : IGlueMapable
    {
        private readonly IJsCsGlue _Glue;
        internal SolvedGlueMapable(IJsCsGlue glue)
        {
            _Glue = glue;
        }

        public object Source => _Glue.CValue;
        public IJsCsGlue Map(IJsUpdateHelper helper) => _Glue;
    }
}
