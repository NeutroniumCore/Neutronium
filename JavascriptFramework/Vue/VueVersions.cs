using Neutronium.Core.Infra;
using Neutronium.Core.JavascriptFramework;

namespace Neutronium.JavascriptFramework.Vue
{
    internal class VueVersions : IVueVersion
    {
        public string Name { get; }
        public string VueVersion { get; }
        public WindowInformation ToolBar { get; }
        public WindowInformation About { get; }
        public string FrameworkNameVersion { get; }

        internal static VueVersions Vue1 { get; } = new VueVersions("1.0.25", "VueInjector", "vue1", null, null);

        internal static VueVersions Vue2 { get; } = new VueVersions("2.2.1", "VueInjectorV2", "vue2",
                                                                        new WindowInformation
                                                                        {
                                                                            RelativePath = "DebugTools\\Toolbar\\index.html",
                                                                        },
                                                                        new WindowInformation
                                                                        {
                                                                            RelativePath = "DebugTools\\About\\index.html",
                                                                            Height = 600,
                                                                            Width = 310
                                                                        });

        public ResourceReader GetVueResource()
        {
            return new ResourceReader($"scripts.{VueVersion}", this);
        }

        private VueVersions(string framewokVersion, string name, string version, WindowInformation toolBar, WindowInformation about)
        {
            FrameworkNameVersion = framewokVersion;
            Name = name;
            VueVersion = version;
            ToolBar = toolBar;
            About = about;
        }
    }
}
