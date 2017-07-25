using System;

namespace Neutronium.Core.Binding.Listeners
{
    internal class Silenter<T> : IDisposable where T: class
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

    internal static class Silenter
    {
        public static Silenter<T> GetSilenter<T>(IEntityUpdater<T> listenerRegister, object target) where T : class
        {
            return new Silenter<T>(listenerRegister, target);
        }
    }
}
