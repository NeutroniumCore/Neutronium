using System;
using Neutronium.Core.Binding.Updaters;
using Neutronium.Core.Infra;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.Listeners
{
    internal class FullListenerRegister
    {
        public ObjectChangesListener On { get; }
        public ObjectChangesListener Off { get; }

        private readonly IJsUpdaterFactory _JsUpdaterFactory;

        private readonly IEntityUpdater<INotifyPropertyChanged> _Property;
        private readonly IEntityUpdater<INotifyCollectionChanged> _Collection;

        private Chained<IJavascriptUpdater> _First;
        private Chained<IJavascriptUpdater> _Last;
        private ReplayMode _ReplayMode = ReplayMode.NotReady;

        private bool QueueIsEmpty => _First == null;

        private enum ReplayMode
        {
            NotReady,
            NoReplayScheduled,
            ReplayScheduled,
            Replaying
        };


        public FullListenerRegister(IJsUpdaterFactory jsUpdaterFactory)
        {
            _JsUpdaterFactory = jsUpdaterFactory;

            _JsUpdaterFactory.OnJavascriptSessionReady += _JsUpdaterFactory_OnJavascriptSessionReady;
            _Property = new ListenerRegister<INotifyPropertyChanged>(n => n.PropertyChanged += OnCSharpPropertyChanged, n => n.PropertyChanged -= OnCSharpPropertyChanged);
            _Collection = new ListenerRegister<INotifyCollectionChanged>(n => n.CollectionChanged += OnCSharpCollectionChanged, n => n.CollectionChanged -= OnCSharpCollectionChanged);
            var command = new ListenerRegister<ICommand>(c => c.CanExecuteChanged += OnCommandCanExecuteChanged, c => c.CanExecuteChanged -= OnCommandCanExecuteChanged);
            var updatableCommand = new ListenerRegister<IUpdatableCommand>(c => c.CanExecuteChanged += OnCommandCanExecuteChanged, c => c.CanExecuteChanged -= OnCommandCanExecuteChanged);

            On = new ObjectChangesListener(_Property.OnEnter, _Collection.OnEnter, command.OnEnter, updatableCommand.OnEnter);
            Off = new ObjectChangesListener(_Property.OnExit, _Collection.OnExit, command.OnExit, updatableCommand.OnExit);
        }

        private void _JsUpdaterFactory_OnJavascriptSessionReady(object sender, System.EventArgs e)
        {
            Debug.Assert(_JsUpdaterFactory.isInUiContext);
            ReplayChanges();
        }

        private void ReplayChanges()
        {
            _ReplayMode = ReplayMode.Replaying;
            DoReplayChanges();
            _ReplayMode = ReplayMode.NoReplayScheduled;
        }

        private void DoReplayChanges()
        {
            if (QueueIsEmpty)
                return;

            var from = _First;
            _JsUpdaterFactory.CheckUiContext();

            bool Agregate(bool value, bool res) => res || value;

            bool ComputeOnUiThread(IJavascriptUpdater updater)
            {
                updater.OnUiContext(Off);
                return updater.NeedToRunOnJsContext;
            }

            var needUpdateOnJsContext = from.Reduce(ComputeOnUiThread, Agregate);
            ResetQueue();
            if (!needUpdateOnJsContext)
                return;

            void PerformOnJsContext(IJavascriptUpdater updater)
            {
                if (!updater.NeedToRunOnJsContext) return;
                updater.OnJsContext();
            }

            _JsUpdaterFactory.DispatchInJavascriptContext(() =>
            {
                @from.ForEach(PerformOnJsContext);
            });
        }

        internal void OnCSharpPropertyChanged(object sender, string propertyName)
        {
            OnCSharpPropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
        }

        private void OnCSharpPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var updater = _JsUpdaterFactory.GetUpdaterForPropertyChanged(sender, e);
            ScheduleChanges(updater);
        }

        private void OnCSharpCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var updater = _JsUpdaterFactory.GetUpdaterForNotifyCollectionChanged(sender, e);
            ScheduleChanges(updater);
        }

        private void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            var updater = _JsUpdaterFactory.GetUpdaterForExcecutionChanged(sender);
            ScheduleChanges(updater);
        }

        public void ScheduleChanges(IJavascriptUpdater updater)
        {
            _JsUpdaterFactory.CheckUiContext();
            Enqueue(updater);

            if (_ReplayMode != ReplayMode.NoReplayScheduled)
                return;

            _ReplayMode = ReplayMode.ReplayScheduled;
            _JsUpdaterFactory.DispatchInUiContextBindingPriority(ReplayChanges);         
        }

        private void Enqueue(IJavascriptUpdater updater)
        {
            _Last = new Chained<IJavascriptUpdater>(updater, _Last);
            _First = _First ?? _Last;
        }

        private void ResetQueue()
        {
            _First = _Last = null;
        }

        public Silenter<INotifyCollectionChanged> GetColllectionSilenter(object target)
        {
            return Silenter.GetSilenter(_Collection, target);
        }

        public PropertyChangedSilenter GetPropertySilenter(object target, string propertyName)
        {
            return Silenter.GetSilenter(_Property, target, propertyName);
        }
    }
}
