using System;
using Neutronium.Core.Binding.GlueObject;
using MoreCollection.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Binding.Listeners
{
    internal class ReListener : IDisposable
    {
        private readonly IUpdatableJSCSGlueCollection _UpdatableCollection;
        private readonly ISet<IJSCSGlue> _Old;
        private bool _Disposed = false;

        public ReListener(IUpdatableJSCSGlueCollection updater)
        {
            _UpdatableCollection = updater;
            _Old = _UpdatableCollection.GetAllChildren();
        }

        public void ForExceptDo(ISet<IJSCSGlue> @for, ISet<IJSCSGlue> except, Action<IJSCSGlue> @do)
        {
            @for.Where(o => !except.Contains(o)).ForEach(@do);
        }

        public void Dispose()
        {
            if (_Disposed)
                return;

            _Disposed = true;
            var @new = _UpdatableCollection.GetAllChildren();
            ForExceptDo(_Old, @new, _UpdatableCollection.OnExit);
            ForExceptDo(@new, _Old, _UpdatableCollection.OnEnter);
        }
    }
}
