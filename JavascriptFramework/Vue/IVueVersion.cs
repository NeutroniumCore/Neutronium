using Neutronium.Core.Infra;
using Neutronium.Core.JavascriptFramework;

namespace Neutronium.JavascriptFramework.Vue
{
    public interface IVueVersion
    {
        string FrameworkNameVersion { get; }

        string Name { get; }

        DebugToolsUI DebugToolsUI { get; }

        ResourceReader GetVueResource();
    }
}
