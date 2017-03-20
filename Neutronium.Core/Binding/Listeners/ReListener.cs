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

            _DeltaProperty = new DeltaListener<INotifyPropertyChanged>();
            _DeltaCollection = new DeltaListener<INotifyCollectionChanged>();
            _DeltaCommand = new DeltaListener<JSCommand>();

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

            _DeltaProperty.Apply(_FullListenerRegister.Property);
            _DeltaCollection.Apply(_FullListenerRegister.Collection);
            _DeltaCommand.Apply(_FullListenerRegister.Command);

            _OnDisposeOk();
        }
    }
}
