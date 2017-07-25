using System;
using Neutronium.Core.Binding.GlueObject;
using MoreCollection.Extensions;
using System.Collections.Generic;
using System.Linq;
using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Listeners
{
    internal class ReListener : IDisposable
    {
        private BridgeUpdater BridgeUpdater { get; set; }

        private readonly IUpdatableJSCSGlueCollection _UpdatableCollection;
        private readonly ISet<IJSCSGlue> _Old;
        private readonly List<IJavascriptObject> _EntityToUnlisten = new List<IJavascriptObject>();
        private bool _Disposed = false;

        public ReListener(IUpdatableJSCSGlueCollection updater)
        {
            _UpdatableCollection = updater;
            _Old = _UpdatableCollection.GetAllChildren();
        }

        public void Dispose()
        {
            if (_Disposed)
                return;

            _Disposed = true;
            var @new = _UpdatableCollection.GetAllChildren();
            ForExceptDo(_Old, @new, OnExitingGlue);
            ForExceptDo(@new, _Old, _UpdatableCollection.OnEnter);

            UpdateUpdater();
        }

        private void UpdateUpdater()
        {
            if ((BridgeUpdater == null) || (_EntityToUnlisten.Count == 0))
                return;

            BridgeUpdater.AddAction(updater => updater.UnListen(_EntityToUnlisten));
        }

        private void OnExitingGlue(IJSCSGlue exiting)
        {
            _UpdatableCollection.OnExit(exiting);
            if (exiting.Type != JsCsGlueType.Object)
                return;

            if (!exiting.CValue.GetType().HasReadWriteProperties())
                return;

            _EntityToUnlisten.Add(exiting.JSValue);
        }

        private void ForExceptDo(ISet<IJSCSGlue> @for, ISet<IJSCSGlue> except, Action<IJSCSGlue> @do)
        {
            @for.Where(o => !except.Contains(o)).ForEach(@do);
        }
    }
}
