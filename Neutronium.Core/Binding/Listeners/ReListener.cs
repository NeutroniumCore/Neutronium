using System;
using Neutronium.Core.Binding.GlueObject;
using MoreCollection.Extensions;

namespace Neutronium.Core.Binding.Listeners
{
    internal class ReListener : IDisposable
    {
        private readonly IJavascriptSessionCache _Cache;
        private readonly IVisitable _Visitable;
        private readonly DeltaListener<IJSCSGlue> _DeltaGlues;
        private readonly Action _OnDisposeOk;
        private readonly FullListenerRegister _FullListenerRegister;
        private int _Count = 1;

        public ReListener(IVisitable visitable, IJavascriptSessionCache cache, FullListenerRegister fullListenerRegister, Action onDispose)
        {
            _Visitable = visitable;
            _Cache = cache;
            _FullListenerRegister = fullListenerRegister;
            _OnDisposeOk = onDispose;

            _DeltaGlues = new DeltaListener<IJSCSGlue>();

            Visit(_DeltaGlues.VisitOld);
        }

        private void Visit(Action<IJSCSGlue> onGlue)
        {
            _Visitable.GetAllChildren().ForEach(onGlue);
        }

        public ReListener AddRef()
        {
            _Count++;
            return this;
        }

        public void Dispose()
        {
            if (--_Count == 0)
                Clean();
        }

        private void Clean()
        {
            Visit(_DeltaGlues.VisitNew);

            var on = _FullListenerRegister.GetOn();
            var off = _FullListenerRegister.GetOff();

            var listenerRegister = new ListenerRegister<IJSCSGlue>(
                item => item.ApplyOnSingleListenable(on) ,
                item =>
                {
                    item.ApplyOnSingleListenable(off);
                    _Cache.Remove(item.CValue);
                });

            _DeltaGlues.Apply(listenerRegister);

            _OnDisposeOk();
        }
    }
}
