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
    internal class CSharpListenerJavascriptUpdater : ICSharpChangesListener
    {
        public ObjectChangesListener On { get; }
        public ObjectChangesListener Off { get; }

        private readonly IJsUpdaterFactory _JsUpdaterFactory;

        private readonly IEntityUpdater<INotifyPropertyChanged> _Property;
        private readonly IEntityUpdater<INotifyCollectionChanged> _Collection;

        private Chained<IJavascriptUIContextUpdater> _First;
        private Chained<IJavascriptUIContextUpdater> _Last;
        private ReplayMode _ReplayMode = ReplayMode.NotReady;

        private bool QueueIsEmpty => _First == null;

        private enum ReplayMode
        {
            NotReady,
            NoReplayScheduled,
            ReplayScheduled,
            Replaying
        };

        public CSharpListenerJavascriptUpdater(IJsUpdaterFactory jsUpdaterFactory)
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

            _JsUpdaterFactory.CheckUiContext();

            IJavascriptJsContextUpdater ExecuteOnUiContext(IJavascriptUIContextUpdater updater) => updater.ExecuteOnUiContext(Off);

            var jsContextUpdated = _First.MapFilter(ExecuteOnUiContext, up => up !=null);
            ResetQueue();
            if (jsContextUpdated == null)
                return;

            _JsUpdaterFactory.DispatchInJavascriptContext(() => jsContextUpdated.ForEach(updater => updater.ExecuteOnJsContext()));
        }

        public void ReportPropertyChanged(object sender, string propertyName)
        {
            var updater = _JsUpdaterFactory.GetUpdaterForPropertyChanged(sender, propertyName);
            ScheduleChanges(updater);
        }

        private void OnCSharpPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            ReportPropertyChanged(sender, propertyChangedEventArgs.PropertyName);
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

        private void ScheduleChanges(IJavascriptUIContextUpdater uiContextUpdater)
        {
            _JsUpdaterFactory.CheckUiContext();
            Enqueue(uiContextUpdater);

            if (_ReplayMode != ReplayMode.NoReplayScheduled)
                return;

            _ReplayMode = ReplayMode.ReplayScheduled;
            _JsUpdaterFactory.DispatchInUiContextBindingPriority(ReplayChanges);         
        }

        private void Enqueue(IJavascriptUIContextUpdater uiContextUpdater)
        {
            _Last = new Chained<IJavascriptUIContextUpdater>(uiContextUpdater, _Last);
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
