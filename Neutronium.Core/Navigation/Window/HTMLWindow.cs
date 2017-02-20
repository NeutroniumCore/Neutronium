using System.Threading.Tasks;
using System.Windows.Input;
using Neutronium.Core.Infra;
using Neutronium.Core.Infra.VM;

namespace Neutronium.Core.Navigation.Window
{
    public class HTMLLogicWindow : NotifyPropertyChangedBase
    {
        private static int _GId = 0;
        private readonly int _Id;
        private TaskCompletionSource<object> _OpenTask = new TaskCompletionSource<object>();
        private TaskCompletionSource<object> _CloseTask = new TaskCompletionSource<object>();

        private WindowLogicalState _State;
        public WindowLogicalState State
        {
            get { return _State; }
            set { Set(ref _State, value); }
        }

        private bool _IsLiteningOpen;
        public bool IsListeningOpen
        {
            get { return _IsLiteningOpen; }
            set { Set(ref _IsLiteningOpen, value); }
        }

        private bool _IsListeningClose;
        public bool IsListeningClose 
        {
            get { return _IsListeningClose; }
            set { Set(ref _IsListeningClose, value); }
        }

        public ICommand CloseReady { get; }
        public ICommand EndOpen { get; }

        internal HTMLLogicWindow()
        {
            _Id = _GId++;
            _State = WindowLogicalState.Loading;
            _IsListeningClose = false;
            CloseReady = new BasicRelayCommand(() => EnTask(_CloseTask));
            EndOpen = new BasicRelayCommand(() => EnTask(_OpenTask));
        }

        private void EnTask(TaskCompletionSource<object> completionSource)
        {
            completionSource.TrySetResult(null);
        }

        public Task OpenAsync() 
        {
            return !IsListeningClose ? TaskHelper.Ended() : _OpenTask.Task;
        }

        public Task CloseAsync()
        {
            if (!IsListeningClose)
                return TaskHelper.Ended();

            State = WindowLogicalState.Closing;
            return _CloseTask.Task;
        }
    }
}
