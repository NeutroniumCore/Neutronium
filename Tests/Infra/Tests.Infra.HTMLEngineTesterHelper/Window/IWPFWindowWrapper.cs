using System;
using System.Threading;

namespace Tests.Infra.HTMLEngineTesterHelper.Window 
{
    public interface IWPFWindowWrapper : IDisposable 
    {
        Thread UIThread { get; }

        System.Windows.Window MainWindow { get; }

        void CloseWindow();
    }
}
