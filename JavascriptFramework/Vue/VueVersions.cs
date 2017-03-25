using Neutronium.Core.Infra;
using Neutronium.Core.JavascriptFramework;

namespace Neutronium.JavascriptFramework.Vue
{
    internal class VueVersions : IVueVersion
    {
        public string Name { get; }
        public string VueVersion { get; }
        public DebugToolsUI DebugToolsUI { get; }
        public string FrameworkNameVersion { get; }

        internal static VueVersions Vue1 { get; } = new VueVersions("1.0.25", "VueInjector", "vue1", null);

        internal static VueVersions Vue2 { get; } = new VueVersions("2.2.5", "VueInjectorV2", "vue2", Vue2DebugToolsUI);

        private static DebugToolsUI Vue2DebugToolsUI => new DebugToolsUI(Vue2DebugTools, Vue2About);

        private static WindowInformation Vue2DebugTools => new WindowInformation
        {
            RelativePath = "DebugTools\\Toolbar\\index.html",
        };

        private static WindowInformation Vue2About => new WindowInformation
        {
            RelativePath = "DebugTools\\About\\index.html",
            Height = 600,
            Width = 310
        };

        public ResourceReader GetVueResource()
        {
            return new ResourceReader($"scripts.{VueVersion}", this);
        }

        private VueVersions(string framewokVersion, string name, string version, DebugToolsUI debugToolsUI)
        {
            FrameworkNameVersion = framewokVersion;
            Name = name;
            VueVersion = version;
            DebugToolsUI = debugToolsUI;
        }
    }
}
