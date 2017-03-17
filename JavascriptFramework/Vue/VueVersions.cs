using Neutronium.Core.Infra;

namespace Neutronium.JavascriptFramework.Vue
{
    internal class VueVersions : IVueVersion
    {
        public string Name { get; }
        public string VueVersion { get; }
        public string ToolBarPath { get;  }
        public string AboutPath { get; }
        public string FrameworkNameVersion { get; }

        internal static VueVersions Vue1 { get; } = new VueVersions("1.0.25", "VueInjector", "vue1", null, null);

        internal static VueVersions Vue2 { get; } = new VueVersions("2.2.1", "VueInjectorV2", "vue2", "DebugTools\\Toolbar\\index.html", "DebugTools\\About\\index.html");

        public ResourceReader GetVueResource()
        {
            return new ResourceReader($"scripts.{VueVersion}", this);
        }

        private VueVersions(string framewokVersion, string name, string version, string path, string aboutPath)
        {
            FrameworkNameVersion = framewokVersion;
            Name = name;
            VueVersion = version;
            ToolBarPath = path;
            AboutPath = aboutPath;
        }
    }
}
