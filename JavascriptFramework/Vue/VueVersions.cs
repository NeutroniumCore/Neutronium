using Neutronium.Core.Infra;
using Neutronium.Core.JavascriptFramework;

namespace Neutronium.JavascriptFramework.Vue
{
    internal class VueVersion
    {
        public string Name { get; }
        public string Version { get; }
        public bool SupportRuntime { get; }
        public DebugToolsUI DebugToolsUI { get; }
        public string FrameworkNameVersion { get; }

        internal static VueVersion Vue1 { get; } = new VueVersion("1.0.25", "VueInjector", "vue1", false, null);

        internal static VueVersion Vue2 { get; } = new VueVersion("2.5.13", "VueInjectorV2", "vue2", true, Vue2DebugToolsUI);

        private static DebugToolsUI Vue2DebugToolsUI => new DebugToolsUI(Vue2DebugTools, Vue2About);

        private static WindowInformation Vue2DebugTools => new WindowInformation
        {
            RelativePath = "DebugTools\\Toolbar\\index.html",
            Height = 65
        };

        private static WindowInformation Vue2About => new WindowInformation
        {
            RelativePath = "DebugTools\\About\\index.html",
            Height = 630,
            Width = 310
        };

        public ResourceReader GetVueResource()
        {
            return new ResourceReader($"scripts.{Version}", this);
        }

        private VueVersion(string framewokVersion, string name, string version, bool supportRuntime, DebugToolsUI debugToolsUI)
        {
            FrameworkNameVersion = framewokVersion;
            Name = name;
            Version = version;
            DebugToolsUI = debugToolsUI;
            SupportRuntime = supportRuntime;
        }
    }
}
