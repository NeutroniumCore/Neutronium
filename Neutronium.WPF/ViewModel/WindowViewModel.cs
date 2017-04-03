using Neutronium.MVVMComponents;
using Neutronium.MVVMComponents.Relay;
using System;
using System.Windows;
using System.ComponentModel;

namespace Neutronium.WPF.ViewModel
{
    public class WindowViewModel : IWindowViewModel
    {
        public ISimpleCommand Close { get; }
        public ISimpleCommand Minimize { get; }
        public ISimpleCommand Maximize { get; }
        public ISimpleCommand Normalize { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        public WindowState State
        {
            get { return _Window.WindowState; }
            set { _Window.WindowState = value; }
        }

        private readonly Window _Window;

        public WindowViewModel(Window window)
        {
            _Window = window;
            Close = new RelaySimpleCommand(() => _Window.Close());
            Minimize = new RelaySimpleCommand(() => State = WindowState.Minimized);
            Maximize = new RelaySimpleCommand(() => State = WindowState.Maximized);
            Normalize = new RelaySimpleCommand(() => State = (State == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized);

            _Window.StateChanged += StateChanged;
        }

        private void StateChanged(object sender, EventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(State)));
        }

        public void Dispose()
        {
            _Window.StateChanged -= StateChanged;
        }
    }
}
