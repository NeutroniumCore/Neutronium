using System;
using System.Collections.Specialized;
using System.ComponentModel;
using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Binding.Listeners
{
    internal class ReListener : IDisposable
    {
        private readonly DeltaListener<INotifyPropertyChanged> _DeltaProperty;
        private readonly DeltaListener<INotifyCollectionChanged> _DeltaCollection;
        private readonly DeltaListener<JSCommand> _DeltaCommand;
        private readonly Action _OnDisposeOk;
        private readonly IVisitable _Visitable;
        private readonly FullListenerRegister _FullListenerRegister;
        private int _Count = 1;

        public ReListener(IVisitable bidirectionalMapper, FullListenerRegister fullListenerRegister, Action onDispose)
        {
            _Visitable = bidirectionalMapper;
            _FullListenerRegister = fullListenerRegister;
            _OnDisposeOk = onDispose;

            _DeltaProperty = DeltaListener.GetDeltaListener(_FullListenerRegister.Property);
            _DeltaCollection = DeltaListener.GetDeltaListener(_FullListenerRegister.Collection);
            _DeltaCommand = DeltaListener.GetDeltaListener(_FullListenerRegister.Command);

            Visit(_DeltaProperty.VisitOld, _DeltaCollection.VisitOld, _DeltaCommand.VisitOld);
        }

        private void Visit(Action<INotifyPropertyChanged> property, Action<INotifyCollectionChanged> collection,
                            Action<JSCommand> command )
        {
            var visitor = new ListenableVisitor(property, collection, command);
            _Visitable.Visit(visitor);
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
            Visit(_DeltaProperty.VisitNew, _DeltaCollection.VisitNew, _DeltaCommand.VisitNew);

            _DeltaProperty.Apply();
            _DeltaCollection.Apply();
            _DeltaCommand.Apply();

            _OnDisposeOk();
        }
    }
}
