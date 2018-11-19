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
        private readonly CounterViewModel _Counter;

        public MainWindow()
        {
            _Counter = new CounterViewModel(50);
            DataContext = _Counter;
            InitializeComponent();
            _Counter.Progess = new Progress<int>(i => _Counter.Counter = i);
        }

        protected override void OnClosed(EventArgs e)
        {
            this.wcBrowser.Dispose();
            base.OnClosed(e);
        }
    }
}
