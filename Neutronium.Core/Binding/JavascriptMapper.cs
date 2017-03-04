using System;
using System.Linq;
using System.Threading.Tasks;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding
{
    internal class JavascriptMapper : IJavascriptObjectInternalMapper
    {
        private readonly IJSObservableBridge _Root;
        private readonly TaskCompletionSource<object> _TCS = new TaskCompletionSource<object>();
        private readonly Action<IJSObservableBridge, IJavascriptObject> _Update;
        private readonly Action<IJavascriptObject, string, IJavascriptObject> _RegisterMapping;
        private readonly Action<IJavascriptObject, string, int, IJavascriptObject> _RegisterCollectionMapping;
        private readonly Action<IJavascriptObject, IJSObservableBridge> _Register;
        private bool? _AutoMapMode;

        public JavascriptMapper(IJSObservableBridge root, Action<IJavascriptObject, IJSObservableBridge> register, Action<IJSObservableBridge, IJavascriptObject> update,
            Action<IJavascriptObject, string, IJavascriptObject> registerMapping, Action<IJavascriptObject, string, int, IJavascriptObject> registerCollectionMapping)
        {
            _Root = root;
            _Register = register;
            _Update = update;
            _RegisterMapping = registerMapping;
            _RegisterCollectionMapping = registerCollectionMapping;
        }

        private void CheckMode(bool mode)
        {
            if (_AutoMapMode == null) 
            {
                _AutoMapMode = mode;
                return;
            }

            if (mode!= _AutoMapMode.Value)
                throw new ArgumentException("When autoMap is called, any other methods can be called");
        }

        public void AutoMap()
        {
            CheckMode(true);
            var observables = _Root.GetAllChildren(true).OfType<IJSObservableBridge>().ToList();
            observables.ForEach(ch =>
            {
                ch.AutoMap();
                _Register(ch.MappedJSValue, ch);
            });
            _TCS.TrySetResult(null);
        }

        public void MapFirst(IJavascriptObject root)
        {
            CheckMode(false);
            _Update(_Root, root);
        }

        public void Map(IJavascriptObject father, string att, IJavascriptObject child)
        {
            CheckMode(false);
            _RegisterMapping(father, att, child);
        }

        public void MapCollection(IJavascriptObject father, string att, int index, IJavascriptObject child)
        {
            CheckMode(false);
            _RegisterCollectionMapping(father, att, index, child);
        }

        public Task UpdateTask => _TCS.Task;

        public void EndMapping(IJavascriptObject root)
        {
            CheckMode(false);
            _TCS.TrySetResult(null);
        }
    }
}
