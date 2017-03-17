using System;
using Neutronium.Core.Infra;

namespace Neutronium.JavascriptFramework.Vue
{
    internal class VueVersions : IVueVersion
    {
        public string FrameworkName { get; }
        public string Name { get; }
        public string VueVersion { get; }
        public string ToolBarPath { get;  }
        public string FrameworkNameVersion { get; }

        internal static VueVersions Vue1 { get; } = new VueVersions("vue.js", "1.0.25", "VueInjector", "vue1", null);

        internal static VueVersions Vue2 { get; } = new VueVersions("vue.js", "2.2.1", "VueInjectorV2", "vue2", "DebugTools\\Toolbar\\index.html");

        public ResourceReader GetVueResource()
        {
            return new ResourceReader($"scripts.{VueVersion}", this);
        }

        private VueVersions(string frameworkName, string framewokVersion, string name, string version, string path)
        {
            FrameworkName = frameworkName;
            FrameworkNameVersion = framewokVersion;
            Name = name;
            VueVersion = version;
            ToolBarPath = path;
        }
    }
}
