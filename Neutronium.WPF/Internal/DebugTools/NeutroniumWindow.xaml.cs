using System;
using System.Windows;

namespace Neutronium.WPF.Internal.DebugTools
{
    /// <summary>
    /// Interaction logic for ViewModelDebug.xaml
    /// </summary>
    public partial class NeutroniumWindow : IDisposable 
    {
        public event EventHandler OnFirstLoad;

        private HTMLViewControl _View;

        public NeutroniumWindow()
        {
            InitializeComponent();
        }

        public NeutroniumWindow(string path, string engine) 
        {
            _View = new HTMLViewControl 
            {
                IsDebug = false,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Uri = new Uri(path),
                JavascriptUIEngine = engine,
                Name = "wcBrowser"
            };
            _View.OnFirstLoad += _View_OnFirstLoad;

            InitializeComponent();
            Main.Children.Add(_View);
        }

        private void _View_OnFirstLoad(object sender, EventArgs e)
        {
            OnFirstLoad?.Invoke(this, e);
        }

        protected override void OnClosed(EventArgs e) 
        {
            base.OnClosed(e);
            Dispose();
        }

        public void Dispose()
        {
            _View?.Dispose();
            _View = null;
        }
    }
}
