using System;
using System.Threading;

namespace Tests.Infra.WebBrowserEngineTesterHelper.Window 
{
    public interface IWPFWindowWrapper : IDisposable 
    {
        Thread UIThread { get; }

        System.Windows.Window MainWindow { get; }

        void CloseWindow();
    }
}
