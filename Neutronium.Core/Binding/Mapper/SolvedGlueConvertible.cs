using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Updater;

namespace Neutronium.Core.Binding.Mapper
{
    internal struct SolvedGlueConvertible : IGlueConvertible
    {
        private readonly IJsCsGlue _Glue;
        internal SolvedGlueConvertible(IJsCsGlue glue)
        {
            _Glue = glue;
        }

        public object Source => _Glue.CValue;
        public IJsCsGlue Convert(IJsUpdateHelper helper) => _Glue;
    }
}
