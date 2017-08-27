using System;
using Neutronium.Core.Binding.GlueObject;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.Listeners
{
    internal class ReListener : IExitContext
    {
        private readonly IUpdatableJsCsGlueCollection _UpdatableGlueCollection;
        private readonly ISet<IJsCsGlue> _Old;
        private bool _Disposed = false;
        private BridgeUpdater _BridgeUpdater;

        public ReListener(IUpdatableJsCsGlueCollection updatableGlueCollection, BridgeUpdater updater)
        {
            _UpdatableGlueCollection = updatableGlueCollection;
            _Old = _UpdatableGlueCollection.GetAllChildren();
            _BridgeUpdater = updater;
        }

        public void SetBridgeUpdater(BridgeUpdater bridgeUpdater)
        {
            _BridgeUpdater = bridgeUpdater;
        }

        public void Dispose()
        {
            if (_Disposed)
                return;

            _Disposed = true;
            var @new = _UpdatableGlueCollection.GetAllChildren();
            ForExceptDo(_Old, @new, OnExitingGlue);
            ForExceptDo(@new, _Old, _UpdatableGlueCollection.OnEnter);
        }

        private void OnExitingGlue(IJsCsGlue exiting)
        {
            _UpdatableGlueCollection.OnExit(exiting, _BridgeUpdater);
        }

        private static void ForExceptDo(ISet<IJsCsGlue> @for, ISet<IJsCsGlue> except, Action<IJsCsGlue> @do)
        {
            foreach(var element in @for)
            {
                if (!except.Contains(element))
                    @do(element);
            }
        }
    }
}
