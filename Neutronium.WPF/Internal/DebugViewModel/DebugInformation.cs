using System.Windows.Input;
using Neutronium.Core.Infra.VM;

namespace Neutronium.WPF.Internal.DebugViewModel 
{
    public class DebugInformation: NotifyPropertyChangedBase 
    {
        private bool _VmDebug;
        public bool VmDebug 
        {
            get { return _VmDebug; }
            set { Set(ref _VmDebug, value); }
        }

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
            set {
                Set(ref _IsDebuggingVm, value);
                DebugButtonLabel = value ? "Close Debug" : "Inspect Vm";
            }
        }

        public string ComponentName { get; set; }
        public string NeutroniumWPFVersion { get; set; }
        public ICommand DebugBrowser { get; set; }
        public BasicRelayCommand DebugWindow { get; set; }
        public ICommand ShowInfo { get; set; }

        internal void SetVmDebug(bool debugableVm) 
        {
            DebugWindow.Executable = debugableVm;
            VmDebug = debugableVm;
        }
    }
}
