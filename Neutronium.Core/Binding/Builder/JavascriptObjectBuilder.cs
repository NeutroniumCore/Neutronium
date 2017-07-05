using MoreCollection.Extensions;
using Neutronium.Core.Binding;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Builder
{
    internal class JavascriptObjectBuilder : IJavascriptObjectBuilder
    {
        private readonly IWebView _WebView;
        private readonly IJavascriptSessionCache _Cache;
        private readonly List<Action<IJavascriptObject>> _ObjectsRequested = new List<Action<IJavascriptObject>>();
        private readonly List<Action<IJavascriptObject>> _ArraysRequested = new List<Action<IJavascriptObject>>();

        public IJavascriptObjectFactory Factory => _WebView.Factory;
        public IWebView WebView => _WebView;

        public JavascriptObjectBuilder(IWebView webView, IJavascriptSessionCache cache)
        {
            _WebView = webView;
            _Cache = cache;
        }

        public void Cache(object @object, IJSCSGlue glueObject)
        {
            _Cache.CacheLocal(@object, glueObject);
        }

        public void RequestObjectCreation(Action<IJavascriptObject> afterBuild)
        {
            _ObjectsRequested.Add(afterBuild);
        }

        public void RequestArrayCreation(Action<IJavascriptObject> afterBuild)
        {
            _ArraysRequested.Add(afterBuild);
        }

        public void UpdateJavascriptValue(IJSCSGlue root)
        {
            var builders = root.GetAllChildren(true).Select(glue => glue.GetJSBuilder()).ToList();
            builders.ForEach(builder => builder.BuildItSelf(this));
            CreateObjects();
            builders.ForEach(builder => builder.UpdateAfterChildrenCreation(this));
        }

        private void CreateObjects()
        {
            var factory = _WebView.Factory;

            var objectCount = _ObjectsRequested.Count;
            if (objectCount > 0)
            {
                var objects = factory.CreateObjects(true, objectCount).ToList();
                for(var i=0; i< objectCount; i++)
                {
                    var @object = objects[i];
                    var @action = _ObjectsRequested[i];
                    @action(@object);
                }  
                
                _ObjectsRequested.Clear();
            }
            
            foreach (var arrayRequested in _ArraysRequested)
            {
                arrayRequested(factory.CreateArray(0));
            }
            _ArraysRequested.Clear();
        }
    }
}
