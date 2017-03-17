using System;
using System.Windows;

namespace Neutronium.WPF.Internal.DebugTools
{
    /// <summary>
    /// Interaction logic for ViewModelDebug.xaml
    /// </summary>
    public partial class NeutroniumWindow : IDisposable 
    {
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

            InitializeComponent();
            Main.Children.Add(_View);
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
