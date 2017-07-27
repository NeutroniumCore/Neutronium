using System;
using Neutronium.Core.Binding.GlueObject;
using MoreCollection.Extensions;
using System.Collections.Generic;
using System.Linq;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Listeners
{
    internal class ReListener : IExitContext
    {
       

        private readonly IUpdatableJSCSGlueCollection _UpdatableGlueCollection;
        private readonly ISet<IJSCSGlue> _Old;
        private readonly List<IJavascriptObject> _EntityToUnlisten = new List<IJavascriptObject>();
        private bool _Disposed = false;
        private BridgeUpdater _BridgeUpdater;

        public ReListener(IUpdatableJSCSGlueCollection updatableGlueCollection)
        {
            _UpdatableGlueCollection = updatableGlueCollection;
            _Old = _UpdatableGlueCollection.GetAllChildren();
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

            UpdateUpdater();
        }

        private void UpdateUpdater()
        {
            if ((_BridgeUpdater == null) || (_EntityToUnlisten.Count == 0))
                return;

            _BridgeUpdater.AddAction(updater => updater.UnListen(_EntityToUnlisten));
        }

        private void OnExitingGlue(IJSCSGlue exiting)
        {
            _UpdatableGlueCollection.OnExit(exiting, this);
        }

        private void ForExceptDo(ISet<IJSCSGlue> @for, ISet<IJSCSGlue> except, Action<IJSCSGlue> @do)
        {
            @for.Where(o => !except.Contains(o)).ForEach(@do);
        }

        public void AddToUnlisten(IJavascriptObject exiting)
        {
            _EntityToUnlisten.Add(exiting);
        }
    }
}
