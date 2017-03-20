using Neutronium.Core.JavascriptFramework;

namespace Neutronium.WPF
{
    public class HTMLWindowInfo
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string AbsolutePath { get; set; }
        public IJavascriptFrameworkManager Framework { get; set; }
    }
}
