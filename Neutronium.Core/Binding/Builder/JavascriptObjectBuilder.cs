using Neutronium.Core.Binding;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Exceptions;
using Neutronium.Core.Extension;
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

        private void Cache(object @object, IJSCSGlue glueObject)
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
            BulkCreate(count => factory.CreateObjects(true, count), _ObjectsRequested);
            BulkCreate(count => factory.CreateArrays(count), _ArraysRequested);
        }

        private static void BulkCreate(Func<int, IEnumerable<IJavascriptObject>> builder, List<Action<IJavascriptObject>> callBack)
        {
            var objectCount = callBack.Count;
            if (objectCount == 0)
                return;

            var objects = builder(objectCount).ToList();
            for (var i = 0; i < objectCount; i++)
            {
                var @object = objects[i];
                var @action = callBack[i];
                @action(@object);
            }
            callBack.Clear();
        }

        public void RequesBasicObjectCreation(object cValue, IJSCSGlue glueObject, Action<IJavascriptObject> afterBuild)
        {
            if (cValue == null)
            {
                afterBuild(Factory.CreateNull());
                return;
            }

            IJavascriptObject value;
            if (Factory.CreateBasic(cValue, out value))
            {
                afterBuild(value);
                return;
            }

            if (!cValue.GetType().IsEnum)
                throw ExceptionHelper.Get("Algorithm core unexpected behaviour");

            afterBuild(Factory.CreateEnum((Enum)cValue));
            Cache(cValue, glueObject);
        }
    }
}
