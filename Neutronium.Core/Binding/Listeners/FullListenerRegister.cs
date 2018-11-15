using Neutronium.Core.Binding.GlueObject.Executable;
using Neutronium.Core.Binding.Updaters;
using Neutronium.Core.Infra;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Neutronium.Core.Binding.Listeners
{
    internal class FullListenerRegister
    {
        public ObjectChangesListener On { get; }
        public ObjectChangesListener Off { get; }

        private IEntityUpdater<INotifyPropertyChanged> Property { get; }
        private IEntityUpdater<INotifyCollectionChanged> Collection { get; }
        private IEntityUpdater<JsCommandBase> Command { get; }

        private readonly IJsUpdaterFactory _JsUpdaterFactory;

        private bool _ReadyToDispatch = false;
        private Chained<IJavascriptUpdater> _First;
        private Chained<IJavascriptUpdater> _Last;
        private bool QueueIsEmpty => _First == null;

        public FullListenerRegister(IJsUpdaterFactory jsUpdaterFactory)
        {
            _JsUpdaterFactory = jsUpdaterFactory;

            _JsUpdaterFactory.OnJavascriptSessionReady += _JsUpdaterFactory_OnJavascriptSessionReady;
            Property = new ListenerRegister<INotifyPropertyChanged>(n => n.PropertyChanged += OnCSharpPropertyChanged, n => n.PropertyChanged -= OnCSharpPropertyChanged);
            Collection = new ListenerRegister<INotifyCollectionChanged>(n => n.CollectionChanged += OnCSharpCollectionChanged, n => n.CollectionChanged -= OnCSharpCollectionChanged);
            Command = new ListenerRegister<JsCommandBase>(c => c.ListenChanges(), c => c.UnListenChanges());
            On = new ObjectChangesListener(Property.OnEnter, Collection.OnEnter, Command.OnEnter);
            Off = new ObjectChangesListener(Property.OnExit, Collection.OnExit, Command.OnExit);
        }

        private void _JsUpdaterFactory_OnJavascriptSessionReady(object sender, System.EventArgs e)
        {
            ReplayChanges();
            _ReadyToDispatch = true;
        }

        private void ReplayChanges()
        {
            if (QueueIsEmpty)
                return;

            var from = _First;
            ResetQueue();

            _JsUpdaterFactory.CheckUiContext();

            bool Agregate(bool value, bool res) => res || value;

            bool ComputeOnUiThread(IJavascriptUpdater updater)
            {
                updater.OnUiContext(Off);
                return updater.NeedToRunOnJsContext;
            }

            void PerformOnJsContext(IJavascriptUpdater updater)
            {
                if (!updater.NeedToRunOnJsContext) return;
                updater.OnJsContext();
            }

            var needUpdateOnJsContext = from.Reduce(ComputeOnUiThread, Agregate);

            if (!needUpdateOnJsContext)
                return;

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
            ReplayChanges(updater);
        }

        private void OnCSharpCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var updater = _JsUpdaterFactory.GetUpdaterForNotifyCollectionChanged(sender, e);
            ReplayChanges(updater);
        }

        private void ReplayChanges(IJavascriptUpdater updater)
        {
            if (!_ReadyToDispatch)
            {
                Enqueue(updater);
                return;
            }

            _JsUpdaterFactory.CheckUiContext();
            updater.OnUiContext(Off);
            if (!updater.NeedToRunOnJsContext)
                return;

            _JsUpdaterFactory.DispatchInJavascriptContext(updater.OnJsContext);
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
            return Silenter.GetSilenter(Collection, target);
        }

        public PropertyChangedSilenter GetPropertySilenter(object target, string propertyName)
        {
            return Silenter.GetSilenter(Property, target, propertyName);
        }
    }
}
