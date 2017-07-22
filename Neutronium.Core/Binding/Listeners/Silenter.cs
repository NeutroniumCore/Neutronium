using System;

namespace Neutronium.Core.Binding.Listeners
{
    internal struct Silenter<T> : IDisposable where T: class
    {
        private readonly ListenerRegister<T> _ListenerRegister;
        private readonly T _Target;
        public Silenter(ListenerRegister<T> listenerRegister, object target)
        {
            _ListenerRegister = listenerRegister;
            _Target = target as T;
            if (_Target != null)
                _ListenerRegister.Off(_Target);
        }

        public void Dispose()
        {
            if (_Target != null)
                _ListenerRegister.On(_Target);
        }
    }

    internal static class Silenter
    {
        public static Silenter<T> GetSilenter<T>(ListenerRegister<T> listenerRegister, object target) where T : class
        {
            return new Silenter<T>(listenerRegister, target);
        }

        public static Silenter<T> Null<T>() where T : class => new Silenter<T>();
    }
}
