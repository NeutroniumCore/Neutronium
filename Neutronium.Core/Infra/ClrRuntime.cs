using System;

namespace Neutronium.Core.Infra
{
    public static class ClrRuntime
    {
        public static int LohArraySize { get; } = (85000 - 13) / IntPtr.Size;
    }
}
