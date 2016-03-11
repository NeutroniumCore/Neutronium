using System;
using System.Threading;

namespace IntegratedTest.Infra.Window 
{
    public interface IWPFWindowWrapper : IDisposable 
    {
        Thread UIThread { get; }

        System.Windows.Window MainWindow { get; }

        void CloseWindow();
    }
}
