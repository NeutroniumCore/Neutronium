using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Updater;

namespace Neutronium.Core.Binding.Mapper
{
    internal struct RawGlueConvertible : IGlueConvertible
    {
        internal RawGlueConvertible(object source)
        {
            Source = source;
        }

        public object Source { get; }
        public IJsCsGlue Convert(IJsUpdateHelper helper)
        {
            return helper.Map(Source);
        }
    }
}
