using Neutronium.Core.Infra;

namespace Neutronium.JavascriptFramework.Vue
{
    internal class VueVersions : IVueVersion
    {
        public string FrameworkName { get; }
        public string Name { get; }
        public string VueVersion { get; }

        internal static VueVersions Vue1 { get; } = new VueVersions("vue.js 1.0.25", "VueInjector", "vue1");

        internal static VueVersions Vue2 { get; } = new VueVersions("vue.js 2.0.3", "VueInjectorV2", "vue2");

        public ResourceReader GetVueResource()
        {
            return new ResourceReader($"scripts.{VueVersion}", this);
        }

        private VueVersions(string frameworkName, string name, string version)
        {
            FrameworkName = frameworkName;
            Name = name;
            VueVersion = version;
        }
    }
}
