using System;

namespace Neutronium.WPF.Internal 
{
    /// <summary>
    /// Interaction logic for DebugControlNeutronium.xaml
    /// </summary>
    public partial class DebugControlNeutronium  : IDisposable
    {
        public DebugControlNeutronium() 
        {
            InitializeComponent();
        }

        public void Dispose() 
        {
            DebugWindow.Dispose();
        }
    }
}
