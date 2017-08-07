using Neutronium.Example.ViewModel.Counter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            _Counter = new CounterViewModel();
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
