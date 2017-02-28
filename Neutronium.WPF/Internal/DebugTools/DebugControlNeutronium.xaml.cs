using Neutronium.Core.Log;
using System;

namespace Neutronium.WPF.Internal.DebugTools
{
    /// <summary>
    /// Interaction logic for DebugControlNeutronium.xaml
    /// </summary>
    public partial class DebugControlNeutronium  : IDisposable
    {
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
