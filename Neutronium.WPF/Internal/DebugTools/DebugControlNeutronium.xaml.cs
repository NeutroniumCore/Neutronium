using Neutronium.Core.Log;
using System;

namespace Neutronium.WPF.Internal.DebugTools
{
    /// <summary>
    /// Interaction logic for DebugControlNeutronium.xaml
    /// </summary>
    public partial class DebugControlNeutronium  : IDisposable
    {
        public string JavascriptUIEngine { get; set; }
        public DebugControlNeutronium(string path, string javascriptUIEngine)
        {
            JavascriptUIEngine = javascriptUIEngine;
            InitializeComponent();
            DebugWindow.Uri = new Uri(path);
            DebugWindow.WebSessionLogger = new NullLogger();
        }

        public DebugControlNeutronium() 
        {
            InitializeComponent();
            DebugWindow.WebSessionLogger = new NullLogger();
        }

        public void Dispose() 
        {
            DebugWindow.Dispose();
        }
    }
}
