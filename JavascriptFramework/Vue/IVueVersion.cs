using Neutronium.Core.Infra;

namespace Neutronium.JavascriptFramework.Vue
{
    public interface IVueVersion
    {
        string FrameworkName { get; }

        string Name { get; }

        ResourceReader GetVueResource();
    }
}
