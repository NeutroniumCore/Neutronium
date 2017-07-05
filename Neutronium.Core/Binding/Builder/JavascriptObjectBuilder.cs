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
            foreach (var objectRequested in _ObjectsRequested)
            {
                objectRequested(factory.CreateObject(true));
            }
            foreach (var arrayRequested in _ArraysRequested)
            {
                arrayRequested(factory.CreateArray(0));
            }
        }
    }
}
