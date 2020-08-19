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
    internal class ListenerUpdater
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

        public ListenerUpdater(IJsUpdaterFactory jsUpdaterFactory)
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

            bool Or(bool value, bool res) => res || value;

            bool UpdateNeedToRunOnJsContext(IJavascriptUpdater updater)
            {
                updater.OnUiContext(Off);
                return updater.NeedToRunOnJsContext;
            }

            var needUpdateOnJsContext = from.Reduce(UpdateNeedToRunOnJsContext, Or);
            ResetQueue();
            if (!needUpdateOnJsContext)
                return;

            _JsUpdaterFactory.DispatchInJavascriptContext(() => @from.ForEach(updater => updater.OnJsContext()));
        }

        internal void OnCSharpPropertyChanged(object sender, string propertyName)
        {
            var updater = _JsUpdaterFactory.GetUpdaterForPropertyChanged(sender, propertyName);
            ScheduleChanges(updater);
        }

        private void OnCSharpPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            OnCSharpPropertyChanged(sender, propertyChangedEventArgs.PropertyName);
        }

        private void OnCSharpCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var updater = _JsUpdaterFactory.GetUpdaterForNotifyCollectionChanged(sender, e);
            ScheduleChanges(updater);
        }

        private void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            var updater = _JsUpdaterFactory.GetUpdaterForExecutionChanged(sender);
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

        public Silenter<INotifyCollectionChanged> GetCollectionSilenter(object target)
        {
            return Silenter.GetSilenter(_Collection, target);
        }

        public PropertyChangedSilenter GetPropertySilenter(object target, string propertyName)
        {
            return Silenter.GetSilenter(_Property, target, propertyName);
        }
    }
}
