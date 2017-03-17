using Neutronium.Core.Infra.VM;
using System.Windows.Input;

namespace Neutronium.WPF.Internal.DebugTools
{
    public class DebugInformation : NotifyPropertyChangedBase
    {
        private string _DebugButtonLabel;
        public string DebugButtonLabel
        {
            get { return _DebugButtonLabel; }
            set { Set(ref _DebugButtonLabel, value); }
        }

        private bool _IsDebuggingVm;
        public bool IsDebuggingVm
        {
            get { return _IsDebuggingVm; }
            set
            {
                Set(ref _IsDebuggingVm, value);
                DebugButtonLabel = value ? "Close Debug" : "Inspect Vm";
            }
        }

        private bool _IsInspecting;
        public bool IsInspecting
        {
            get { return _IsInspecting; }
            set { Set(ref _IsInspecting, value); }
        }

        public string ComponentName { get; set; }
        public string NeutroniumWPFVersion { get; set; }
        public ICommand DebugBrowser { get; set; }
        public ICommand ShowInfo { get; set; }
        public ICommand SaveVm { get; set; }
        public BasicRelayCommand DebugWindow { get; set; }
    }
}
