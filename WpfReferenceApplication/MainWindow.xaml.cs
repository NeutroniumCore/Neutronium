using Neutronium.Example.ViewModel.Counter;
using System;
using System.Windows;

namespace WpfReferenceApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CounterViewModel _Counter;
        public MainWindow()
        {
            _Counter = new CounterViewModel();
            DataContext = _Counter;
            InitializeComponent();
            _Counter.Progess = new Progress<int>(i => _Counter.Counter = i);
        }
    }
}
