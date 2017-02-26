using System;

namespace Neutronium.Core.Infra
{
    public class DisposableAction : IDisposable
    {
        private Action _Dispose;
        public DisposableAction(Action dispose)
        {
            _Dispose = dispose;
        }

        public void Dispose()
        {
            _Dispose?.Invoke();
            _Dispose = null;
        }
    }
}
