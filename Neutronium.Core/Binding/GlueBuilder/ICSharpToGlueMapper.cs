using System;
using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Binding.GlueBuilder
{
    internal interface ICSharpToGlueMapper
    {
        IJsCsGlue Map(object from);

        bool IsBasicType(Type type);
    }
}
