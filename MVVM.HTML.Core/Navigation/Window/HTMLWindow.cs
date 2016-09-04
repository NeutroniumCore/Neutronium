using System.Threading.Tasks;
using System.Windows.Input;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.Infra.VM;

namespace MVVM.HTML.Core.Navigation.Window
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
            set { Set(ref _State, value, nameof(State)); }
        }

        private bool _IsLiteningOpen;
        public bool IsListeningOpen
        {
            get { return _IsLiteningOpen; }
            set { Set(ref _IsLiteningOpen, value, nameof(IsListeningOpen)); }
        }

        private bool _IsLiteningClose;
        public bool IsListeningClose 
        {
            get { return _IsLiteningClose; }
            set { Set(ref _IsLiteningClose, value, nameof(IsListeningClose)); }
        }

        public ICommand CloseReady { get; }
        public ICommand EndOpen { get; }

        internal HTMLLogicWindow()
        {
            _Id = _GId++;
            _State = WindowLogicalState.Loading;
            _IsLiteningClose = false;
            CloseReady = new BasicRelayCommand(() => EnTask(_CloseTask));
            EndOpen = new BasicRelayCommand(() => EnTask(_OpenTask));
        }

        private void EnTask(TaskCompletionSource<object> completionSource)
        {
            completionSource.TrySetResult(null);
        }

        public Task OpenAsync()
        {
            if (!IsListeningClose)
                return TaskHelper.Ended();

            return _OpenTask.Task;
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
