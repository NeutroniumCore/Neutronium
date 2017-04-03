using Neutronium.MVVMComponents;
using System;
using System.ComponentModel;

namespace Neutronium.WPF.ViewModel
{
    public interface IWindowViewModel : IDisposable, INotifyPropertyChanged
    {
        ISimpleCommand Close { get; }
        ISimpleCommand Minimize { get; }
        ISimpleCommand Maximize { get; }
    }
}