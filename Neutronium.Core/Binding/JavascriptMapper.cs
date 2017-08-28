using System;
using System.Threading.Tasks;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding
{
    internal class JavascriptMapper : IJavascriptObjectInternalMapper
    {
        private readonly IJsCsMappedBridge _Root;
        private readonly TaskCompletionSource<object> _Tcs = new TaskCompletionSource<object>();
        private readonly Action<IJsCsMappedBridge, IJavascriptObject> _Update;
        private readonly Action<IJavascriptObject, string, IJavascriptObject> _RegisterMapping;
        private readonly Action<IJavascriptObject, string, int, IJavascriptObject> _RegisterCollectionMapping;

        public JavascriptMapper(IJsCsMappedBridge root, Action<IJsCsMappedBridge, IJavascriptObject> update,
            Action<IJavascriptObject, string, IJavascriptObject> registerMapping, Action<IJavascriptObject, string, int, IJavascriptObject> registerCollectionMapping)
        {
            _Root = root;
            _Update = update;
            _RegisterMapping = registerMapping;
            _RegisterCollectionMapping = registerCollectionMapping;
        }

        public void MapFirst(IJavascriptObject root)
        {
            _Update(_Root, root);
        }

        public void Map(IJavascriptObject father, string attribute, IJavascriptObject child)
        {
            _RegisterMapping(father, attribute, child);
        }

        public void MapCollection(IJavascriptObject father, string attribute, int index, IJavascriptObject child)
        {
            _RegisterCollectionMapping(father, attribute, index, child);
        }

        public Task UpdateTask => _Tcs.Task;

        public void EndMapping(IJavascriptObject root)
        {
            _Tcs.TrySetResult(null);
        }
    }
}
