using System;
using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Binding.Mapper
{
    internal interface ICSharpToGlueMapper
    {
        IJsCsGlue Map(object from);

        bool IsBasicType(Type type);
    }
}
