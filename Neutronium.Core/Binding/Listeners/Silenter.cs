using System;
using System.ComponentModel;

namespace Neutronium.Core.Binding.Listeners
{
    internal struct Silenter<T> : IDisposable where T: class
    {
        private readonly IEntityUpdater<T> _ListenerRegister;
        private readonly T _Target;
        public Silenter(IEntityUpdater<T> listenerRegister, object target)
        {
            _ListenerRegister = listenerRegister;
            _Target = target as T;
            if (_Target != null)
                _ListenerRegister.OnExit(_Target);
        }

        public void Dispose()
        {
            if (_Target != null)
                _ListenerRegister.OnEnter(_Target);
        }
    }

    internal class PropertyChangedSilenter : IDisposable, INotifyPropertyChanged
    {
        private readonly IEntityUpdater<INotifyPropertyChanged> _ListenerRegister;
        private readonly string _PropertyName;
        private INotifyPropertyChanged _Target;

        public event PropertyChangedEventHandler PropertyChanged;

        public PropertyChangedSilenter(IEntityUpdater<INotifyPropertyChanged> listenerRegister, object target, string propertyName)
        {
            _ListenerRegister = listenerRegister;
            _PropertyName = propertyName;
            _Target = target as INotifyPropertyChanged;
            if (_Target == null)
                return;

            _Target.PropertyChanged += TargetPropertyChanged;
            _ListenerRegister.OnExit(_Target);
            _ListenerRegister.OnEnter(this);
        }

        private void TargetPropertyChanged(object originalSender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _PropertyName)
                return;

            PropertyChanged?.Invoke(originalSender, e);
        }

        public void Dispose()
        {
            if (_Target == null)
                return;

            _Target.PropertyChanged -= TargetPropertyChanged;
            _ListenerRegister.OnExit(this);
            _ListenerRegister.OnEnter(_Target);
            _Target = null;
        }
    }

    internal static class Silenter
    {
        public static Silenter<T> GetSilenter<T>(IEntityUpdater<T> listenerRegister, object target) where T : class
        {
            return new Silenter<T>(listenerRegister, target);
        }

        public static PropertyChangedSilenter GetSilenter(IEntityUpdater<INotifyPropertyChanged> listenerRegister, object target, string propertyName)
        {
            return new PropertyChangedSilenter(listenerRegister, target, propertyName);
        }
    }
}
