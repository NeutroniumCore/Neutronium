using MoreCollection.Extensions;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Exceptions;
using Neutronium.Core.Extension;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Binding.Builder
{
    internal class JavascriptObjectOneShotBuilder
    {
        private readonly IWebView _WebView;
        private readonly IJavascriptSessionCache _Cache;
        private readonly IJSCSGlue _Root;
        private IJavascriptObjectFactory Factory => _WebView.Factory;

        private readonly List<IJSCSGlue> _ObjectsRequested = new List<IJSCSGlue>();
        private readonly List<IJSCSGlue> _ArraysRequested = new List<IJSCSGlue>();
        private readonly List<Tuple<IJSCSGlue, IReadOnlyDictionary<string, IJSCSGlue>>>
                _ObjectsBuildingRequested = new List<Tuple<IJSCSGlue, IReadOnlyDictionary<string, IJSCSGlue>>>();
        private readonly List<Tuple<IJSCSGlue, IList<IJSCSGlue>>>
                _ArraysBuildingRequested = new List<Tuple<IJSCSGlue, IList<IJSCSGlue>>>();


        public JavascriptObjectOneShotBuilder(IWebView webView, IJavascriptSessionCache cache, IJSCSGlue root)
        {
            _WebView = webView;
            _Cache = cache;
            _Root = root;
        }

        public void UpdateJavascriptValue()
        {
            var builders = _Root.GetAllChildren(true).Where(glue => glue.JSValue == null)
                                .Select(glue => new JSBuilderAdapter(glue, this)).ToList();

            builders.ForEach(builder => builder.GetBuildRequest());
            CreateObjects();
            UpdateDependencies();
        }

        internal void RequestObjectCreation(IJSCSGlue glue, IReadOnlyDictionary<string, IJSCSGlue> children)
        {
            _ObjectsRequested.Add(glue);

            if ((children!=null) && (children.Count > 0))
                _ObjectsBuildingRequested.Add(Tuple.Create(glue, children));
        }

        internal void RequestArrayCreation(IJSCSGlue glue, IList<IJSCSGlue> children)
        {
            _ArraysRequested.Add(glue);

            if ((children != null) && (children.Count > 0))
                _ArraysBuildingRequested.Add(Tuple.Create(glue, children));

        }

        internal void RequestBasicObjectCreation(IJSCSGlue glueObject, object cValue)
        {
            if (cValue == null)
            {
                glueObject.SetJSValue(Factory.CreateNull());
                return;
            }

            IJavascriptObject value;
            if (Factory.CreateBasic(cValue, out value))
            {
                glueObject.SetJSValue(value);
                return;
            }

            if (!cValue.GetType().IsEnum)
                throw ExceptionHelper.Get("Algorithm core unexpected behaviour");

            glueObject.SetJSValue(Factory.CreateEnum((Enum)cValue));
            _Cache.CacheLocal(cValue, glueObject);
        }

        internal void RequestCommandCreation(IJSCSGlue glueObject, bool canExcecute)
        {
            var command = Factory.CreateObject(true);
            command.SetValue("CanExecuteValue", Factory.CreateBool(canExcecute));
            command.SetValue("CanExecuteCount", Factory.CreateInt(1));

            glueObject.SetJSValue(command);
        }

        private void UpdateDependencies()
        {
            foreach(var objectUpdate in _ObjectsBuildingRequested)
            {
                var attributes = objectUpdate.Item2;
                var jsValue = objectUpdate.Item1.JSValue;
                attributes.ForEach(attribute => jsValue.SetValue(attribute.Key, attribute.Value.JSValue));
            }

            foreach (var arrayUpdate in _ArraysBuildingRequested)
            {
                var children = arrayUpdate.Item2;
                var jsValue = arrayUpdate.Item1.JSValue;
                var dest = children.Select(v => v.JSValue).ToArray();
                jsValue.InvokeNoResult("push", _WebView, dest);
            }
        }

        private void CreateObjects()
        {
            var factory = _WebView.Factory;
            BulkCreate(count => factory.CreateObjects(true, count), _ObjectsRequested);
            BulkCreate(count => factory.CreateArrays(count), _ArraysRequested);
        }

        private static void BulkCreate(Func<int, IEnumerable<IJavascriptObject>> builder, List<IJSCSGlue> glues)
        {
            var objectCount = glues.Count;
            if (objectCount == 0)
                return;

            var objects = builder(objectCount).ToList();
            for (var i = 0; i < objectCount; i++)
            {
                glues[i].SetJSValue(objects[i]);
            }
        }
    }
}
