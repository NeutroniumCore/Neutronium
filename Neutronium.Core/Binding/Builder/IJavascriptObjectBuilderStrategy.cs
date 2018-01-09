using System;
using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Binding.Builder
{
    public interface IJavascriptObjectBuilderStrategy: IDisposable
    {
        void UpdateJavascriptValue(IJsCsGlue root);
    }
}
