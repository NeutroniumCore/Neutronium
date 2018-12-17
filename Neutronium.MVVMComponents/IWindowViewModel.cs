using System;
using System.ComponentModel;

namespace Neutronium.MVVMComponents
{
    public interface IWindowViewModel : IDisposable, INotifyPropertyChanged
    {
        ISimpleCommand Close { get; }
        ISimpleCommand Minimize { get; }
        ISimpleCommand Maximize { get; }
    }
}
