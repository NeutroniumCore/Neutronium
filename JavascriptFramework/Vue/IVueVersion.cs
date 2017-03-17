using Neutronium.Core.Infra;

namespace Neutronium.JavascriptFramework.Vue
{
    public interface IVueVersion
    {
        string FrameworkName { get; }

        string FrameworkNameVersion { get; }

        string Name { get; }

        string ToolBarPath { get; }

        string AboutPath { get; }

        ResourceReader GetVueResource();
    }
}
