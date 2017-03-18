using Neutronium.Core.Infra;
using Neutronium.Core.JavascriptFramework;

namespace Neutronium.JavascriptFramework.Vue
{
    public interface IVueVersion
    {
        string FrameworkNameVersion { get; }

        string Name { get; }

        WindowInformation ToolBar { get; }

        WindowInformation About { get; }

        ResourceReader GetVueResource();
    }
}
