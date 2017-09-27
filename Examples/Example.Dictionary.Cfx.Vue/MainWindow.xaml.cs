using System;
using System.Dynamic;
using System.Windows;
using Example.Dictionary.Cfx.Vue.ViewModel;

namespace Example.Dictionary.Cfx.Vue
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            dynamic dataContext = new ExpandoObject();
            dataContext.Hello = "world";
            dataContext.Value = 666;

            var vm = new MainViewModel(dataContext);
            DataContext = vm;
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            this.wcBrowser.Dispose();
            base.OnClosed(e);
        }
    }
}
