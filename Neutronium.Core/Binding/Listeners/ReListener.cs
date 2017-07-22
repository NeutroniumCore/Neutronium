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
        private readonly Action _OnDisposeOk;
        private readonly ISet<IJSCSGlue> _Old;
        private int _Count = 1;

        private ReListener(IUpdatableJSCSGlueCollection updater, Action onDispose)
        {
            _UpdatableCollection = updater;
            _OnDisposeOk = onDispose;
            _Old = _UpdatableCollection.GetAllChildren();
        }

        private ReListener AddRef()
        {
            _Count++;
            return this;
        }

        public void Dispose()
        {
            if (--_Count == 0)
                Clean();
        }

        public static ReListener UpdateOrCreate(ref ReListener listener, IUpdatableJSCSGlueCollection updater, Action onDispose)
        {
            return listener = (listener == null) ? new ReListener(updater, onDispose) : listener.AddRef();
        }

        public void ForExceptDo(ISet<IJSCSGlue> @for, ISet<IJSCSGlue> except, Action<IJSCSGlue> @do)
        {
            @for.Where(o => !except.Contains(o)).ForEach(@do);
        }

        private void Clean()
        {
            var @new = _UpdatableCollection.GetAllChildren();
            ForExceptDo(_Old, @new, _UpdatableCollection.OnExit);
            ForExceptDo(@new, _Old, _UpdatableCollection.OnEnter);
            _OnDisposeOk();
        }
    }
}
