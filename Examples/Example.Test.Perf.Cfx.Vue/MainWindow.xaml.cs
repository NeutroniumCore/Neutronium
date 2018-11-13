using Neutronium.Example.ViewModel.Counter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace Example.Test.Perf.Cfx.Vue
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CounterViewModel _Counter;

        public MainWindow()
        {
            _Counter = new CounterViewModel(50);
            DataContext = _Counter;
            InitializeComponent();
            _Counter.Progess = new Progress<int>(i => _Counter.Counter = i);
            var observerDispatch = new ObserverDispatch(_Counter);
            var count = 0;
            observerDispatch.PropertyChanged += (_, n) =>
            {
                Console.WriteLine($"{n.PropertyName} {count++} {_Counter.Counter}");
            };
        }

        protected override void OnClosed(EventArgs e)
        {
            this.wcBrowser.Dispose();
            base.OnClosed(e);
        }
    }

    public class ObserverDispatch: INotifyPropertyChanged
    {
        private bool _Trigered = false;
        private readonly INotifyPropertyChanged _Wrapped;
        private readonly Stack<PropertyChangedEventArgs> _Notifications = new Stack<PropertyChangedEventArgs>();   

        public ObserverDispatch(INotifyPropertyChanged wrapped)
        {
            _Wrapped = wrapped;
            _Wrapped.PropertyChanged += _Wrapped_PropertyChanged;
        }

        private void _Wrapped_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _Notifications.Push(e);
            if (_Trigered)
                return;

            _Trigered = true;
            Delegate dispatch = new Action(Dispatch);
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, dispatch);
        }

        public void Dispatch()
        {
            var notification = _Notifications.Pop();
            _Notifications.Clear();
            PropertyChanged?.Invoke(_Wrapped, notification);
            _Trigered = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
