using System;
using System.Threading;
using System.Windows;

namespace IntegratedTest.WPF.Infra 
{
    public interface IWPFWindowWrapper : IDisposable 
    {
        Thread UIThread { get; }

        Window MainWindow { get; }

        void CloseWindow();
    }
}
