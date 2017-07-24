using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Extension;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Binding.Builder
{
    internal class JavascriptObjectBulkBuilder
    {
        private readonly IJavascriptObjectFactory _Factory;
        private readonly IJavascriptSessionCache _Cache;
        private readonly IBulkUpdater _BulkPropertyUpdater;
        private readonly IJSCSGlue _Root;

        private readonly ObjectsCreationRequest _ObjectsCreationRequest = new ObjectsCreationRequest();
        private readonly List<EntityDescriptor<int>> _ArraysBuildingRequested = new List<EntityDescriptor<int>>();
        private readonly List<IJSCSGlue> _BasicObjectsToCreate = new List<IJSCSGlue>();

        public JavascriptObjectBulkBuilder(IJavascriptObjectFactory factory, IJavascriptSessionCache cache, IBulkUpdater bulkPropertyUpdater, 
            IJSCSGlue root)
        {
            _Factory = factory;
            _Cache = cache;
            _Root = root;
            _BulkPropertyUpdater = bulkPropertyUpdater;
        }

        public void UpdateJavascriptValue()
        {
            var builders = _Root.GetAllChildren(true).Where(glue => glue.JSValue == null)
                                .Select(glue => new JSBuilderAdapter(glue, this)).ToList();

            builders.ForEach(builder => builder.GetBuildRequest());
            CreateObjects();
            UpdateDependencies();
        }

        internal void RequestObjectCreation(IJSCSGlue glue, IReadOnlyDictionary<string, IJSCSGlue> children, bool updatableFromJS)
        {
            _ObjectsCreationRequest.AddRequest(EntityDescriptor.CreateObjectDescriptor(glue, children), updatableFromJS);
        }

        internal void RequestArrayCreation(IJSCSGlue glue, IList<IJSCSGlue> children)
        {
            _ArraysBuildingRequested.Add(EntityDescriptor.CreateArrayDescriptor(glue, children));
        }

        internal void RequestBasicObjectCreation(IJSCSGlue glueObject, object cValue)
        {
            if (cValue == null)
            {
                glueObject.SetJSValue(_Factory.CreateNull());
                return;
            }

            if (cValue.GetType().IsEnum)
            {
                glueObject.SetJSValue(_Factory.CreateEnum((Enum)cValue));
                _Cache.CacheLocal(cValue, glueObject);
                return;
            }

            _BasicObjectsToCreate.Add(glueObject);
        }

        internal void RequestCommandCreation(IJSCSGlue glueObject, bool canExcecute)
        {
            var command = _Factory.CreateObject(true);
            command.SetValue("CanExecuteValue", _Factory.CreateBool(canExcecute));
            command.SetValue("CanExecuteCount", _Factory.CreateInt(1));

            glueObject.SetJSValue(command);
        }

        private void CreateObjects()
        {
            BulkCreate(_ => _Factory.CreateBasics(_BasicObjectsToCreate.Select(glue => glue.CValue).ToList()), _BasicObjectsToCreate);
            BulkCreate(count => _Factory.CreateObjects(_ObjectsCreationRequest.ReadWriteNumber, _ObjectsCreationRequest.ReadOnlyNumber), _ObjectsCreationRequest.GetElements());
            BulkCreate(count => _Factory.CreateArrays(count), _ArraysBuildingRequested.Select(item => item.Father).ToList());
        }

        private void UpdateDependencies()
        {
            UpdateObjects();
            UpdateArrays();  
        }

        private void UpdateObjects()
        {
            var toBeUpdated = _ObjectsCreationRequest.GetElementWithProperty();          
            _BulkPropertyUpdater.BulkUpdateProperty(toBeUpdated);
        }

        private void UpdateArrays()
        {
            var toBeUpdated = _ArraysBuildingRequested.Where(item => item.ChildrenDescription.Length > 0).ToList();
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
