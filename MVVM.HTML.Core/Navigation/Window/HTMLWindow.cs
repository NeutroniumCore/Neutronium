using System.ComponentModel;
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

        private WindowLogicalState _State;
        public WindowLogicalState State
        {
            get { return _State; }
            set { Set(ref _State, value, nameof(State)); }
        }

        private bool _IsLiteningOpen;
        public bool IsLiteningOpen
        {
            get { return _IsLiteningOpen; }
            set { Set(ref _IsLiteningOpen, value, nameof(IsLiteningOpen)); }
        }

        private bool _IsLiteningClose;
        public bool IsListeningClose 
        {
            get { return _IsLiteningClose; }
            set { Set(ref _IsLiteningClose, value, nameof(IsListeningClose)); }
        }

        private TaskCompletionSource<object> _OpenTask;

        internal HTMLLogicWindow()
        {
            _Id = _GId++;
            _State = WindowLogicalState.Loading;
            _IsLiteningClose = false;
            CloseReady = new BasicRelayCommand(() => State = WindowLogicalState.Closed);
            EndOpen = new BasicRelayCommand(() => { _OpenTask?.TrySetResult(null); });
        }

        public Task OpenAsync()
        {
            if (!IsListeningClose)
                return TaskHelper.Ended();

            if (_OpenTask==null)
                _OpenTask = new TaskCompletionSource<object>();

            return _OpenTask.Task;
        }

        public Task CloseAsync()
        {
            if (!IsListeningClose)
                return TaskHelper.Ended();

            var tcs = new TaskCompletionSource<object>();

            PropertyChangedEventHandler echa = null;

            echa = (o, e) =>
                {
                    if (State == WindowLogicalState.Closed)
                    {
                        tcs.SetResult(null);
                        this.PropertyChanged -= echa;
                    }
                };

            this.PropertyChanged += echa;
            State = WindowLogicalState.Closing;

            return tcs.Task;
        }

        public ICommand CloseReady { get; }

        public ICommand EndOpen { get; }
    }
}
