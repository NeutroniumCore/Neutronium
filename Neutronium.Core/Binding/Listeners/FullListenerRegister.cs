using Neutronium.Core.Binding.GlueObject.Executable;
using Neutronium.Core.Binding.Updaters;
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

        private readonly IJsUpdateHelper _JsUpdateHelper;

        public FullListenerRegister(IJsUpdateHelper jsUpdateHelper)
        {
            _JsUpdateHelper = jsUpdateHelper;
            Property = new ListenerRegister<INotifyPropertyChanged>(n => n.PropertyChanged += OnCSharpPropertyChanged, n => n.PropertyChanged -= OnCSharpPropertyChanged);
            Collection = new ListenerRegister<INotifyCollectionChanged>(n => n.CollectionChanged += OnCSharpCollectionChanged, n => n.CollectionChanged -= OnCSharpCollectionChanged);
            Command = new ListenerRegister<JsCommandBase>(c => c.ListenChanges(), c => c.UnListenChanges());
            On = new ObjectChangesListener(Property.OnEnter, Collection.OnEnter, Command.OnEnter);
            Off = new ObjectChangesListener(Property.OnExit, Collection.OnExit, Command.OnExit);
        }

        internal void OnCSharpPropertyChanged(object sender, string propertyName)
        {
            OnCSharpPropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
        }

        private void OnCSharpPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var updater = _JsUpdateHelper.GetUpdaterForPropertyChanged(sender, e);
            ReplayChanges(updater);
        }

        private void OnCSharpCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var updater = _JsUpdateHelper.GetUpdaterForNotifyCollectionChanged(sender, e);
            ReplayChanges(updater);
        }

        private void ReplayChanges(IJavascriptUpdater updater)
        {
            _JsUpdateHelper.CheckUiContext();
            updater.OnUiContext(Off);
            if (!updater.NeedToRunOnJsContext)
                return;

            _JsUpdateHelper.DispatchInJavascriptContext(updater.OnJsContext);
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
