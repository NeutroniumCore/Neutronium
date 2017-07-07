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
        private readonly IJavascriptObjectFactory _Factory;
        private readonly IJavascriptSessionCache _Cache;
        private readonly IBulkUpdater _BulkPropertyUpdater;
        private readonly IJSCSGlue _Root;

        private readonly List<Tuple<IJSCSGlue, IReadOnlyDictionary<string, IJSCSGlue>>>
                _ObjectsBuildingRequested = new List<Tuple<IJSCSGlue, IReadOnlyDictionary<string, IJSCSGlue>>>();
        private readonly List<Tuple<IJSCSGlue, IList<IJSCSGlue>>>
                _ArraysBuildingRequested = new List<Tuple<IJSCSGlue, IList<IJSCSGlue>>>();

        public JavascriptObjectOneShotBuilder(IJavascriptObjectFactory factory, IJavascriptSessionCache cache, IBulkUpdater bulkPropertyUpdater, 
            IJSCSGlue root)
        {
            _Factory = factory;
            _Cache = cache;
            _Root = root;
            _BulkPropertyUpdater = bulkPropertyUpdater;
        }

        internal void RequestObjectCreation(IJSCSGlue glue, IReadOnlyDictionary<string, IJSCSGlue> children)
        {
            _ObjectsBuildingRequested.Add(Tuple.Create(glue, children));
        }

        internal void RequestArrayCreation(IJSCSGlue glue, IList<IJSCSGlue> children)
        {
            _ArraysBuildingRequested.Add(Tuple.Create(glue, children));
        }

        internal void RequestBasicObjectCreation(IJSCSGlue glueObject, object cValue)
        {
            if (cValue == null)
            {
                glueObject.SetJSValue(_Factory.CreateNull());
                return;
            }

            IJavascriptObject value;
            if (_Factory.CreateBasic(cValue, out value))
            {
                glueObject.SetJSValue(value);
                return;
            }

            if (!cValue.GetType().IsEnum)
                throw ExceptionHelper.Get("Algorithm core unexpected behaviour");

            glueObject.SetJSValue(_Factory.CreateEnum((Enum)cValue));
            _Cache.CacheLocal(cValue, glueObject);
        }

        internal void RequestCommandCreation(IJSCSGlue glueObject, bool canExcecute)
        {
            var command = _Factory.CreateObject(true);
            command.SetValue("CanExecuteValue", _Factory.CreateBool(canExcecute));
            command.SetValue("CanExecuteCount", _Factory.CreateInt(1));

            glueObject.SetJSValue(command);
        }

        public void UpdateJavascriptValue()
        {
            var builders = _Root.GetAllChildren(true).Where(glue => glue.JSValue == null)
                                .Select(glue => new JSBuilderAdapter(glue, this)).ToList();

            builders.ForEach(builder => builder.GetBuildRequest());
            CreateObjects();
            UpdateDependencies();
        }

        private void CreateObjects()
        {
            BulkCreate(count => _Factory.CreateObjects(true, count), _ObjectsBuildingRequested.Select(item =>item.Item1).ToList());
            BulkCreate(count => _Factory.CreateArrays(count), _ArraysBuildingRequested.Select(item => item.Item1).ToList());
        }

        private void UpdateDependencies()
        {
            UpdateObjects();
            UpdateArrays();  
        }

        private void UpdateObjects()
        {
            var toBeUpdated = _ObjectsBuildingRequested.Where(item => item.Item2 != null && item.Item2.Count > 0).ToList();
            _BulkPropertyUpdater.BulkUpdateProperty(toBeUpdated);
        }

        private void UpdateArrays()
        {
            var toBeUpdated = _ArraysBuildingRequested.Where(item => item.Item2 != null && item.Item2.Count > 0).ToList();
            _BulkPropertyUpdater.BulkUpdateArray(toBeUpdated);
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
