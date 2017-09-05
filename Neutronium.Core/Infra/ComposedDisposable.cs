using System;
using MoreCollection.Extensions;

namespace Neutronium.Core.Infra
{
    public class ComposedDisposable : IDisposable
    {
        private readonly IDisposable[] _Disposables;
        public ComposedDisposable(params IDisposable[] disposables)
        {
            _Disposables = disposables;
        }

        public void Dispose()
        {
            _Disposables.ForEach(d => d.Dispose());
        }
    }
}
