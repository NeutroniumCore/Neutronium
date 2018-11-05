using System;

namespace Neutronium.Core.Infra.Reflection
{
    [Flags]
    public enum ObjectObservability
    {
        None = 0,
        ReadOnly = 1,
        ImplementNotifyProperty = 2
    }
}
