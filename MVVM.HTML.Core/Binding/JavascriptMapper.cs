using MVVM.HTML.Core.Binding.GlueObject;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;
using System;
using System.Threading.Tasks;

namespace MVVM.HTML.Core.Binding
{
    internal class JavascriptMapper : IJavascriptObjectInternalMapper
    {
        private readonly IJSObservableBridge _Root;
        private readonly TaskCompletionSource<object> _TCS = new TaskCompletionSource<object>();
        private readonly Action<IJSObservableBridge, IJavascriptObject> _Update;
        private readonly Action<IJavascriptObject, string, IJavascriptObject> _RegisterMapping;
        private readonly Action<IJavascriptObject, string, int, IJavascriptObject> _RegisterCollectionMapping;

        public JavascriptMapper(IJSObservableBridge root, Action<IJSObservableBridge, IJavascriptObject> update,
            Action<IJavascriptObject, string, IJavascriptObject> registerMapping, Action<IJavascriptObject, string, int, IJavascriptObject> registerCollectionMapping)
        {
            _Root = root;
            _Update = update;
            _RegisterMapping = registerMapping;
            _RegisterCollectionMapping = registerCollectionMapping;
        }

        public void MapFirst(IJavascriptObject iRoot)
        {         
            _Update(_Root, iRoot);
        }

        public void Map(IJavascriptObject father, string att, IJavascriptObject child)
        { 
            _RegisterMapping(father, att, child);
        }

        public void MapCollection(IJavascriptObject father, string att, int index, IJavascriptObject child)
        {
            _RegisterCollectionMapping(father, att, index, child);
        }

        public Task UpdateTask => _TCS.Task;

        public void EndMapping(IJavascriptObject iRoot)
        {
            _TCS.TrySetResult(null);
        }
    }
}
