using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Extension;
using Neutronium.Core.Infra;
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
        private readonly bool _NeedToCacheObject;

        private readonly ObjectsCreationRequest _ObjectsCreationRequest = new ObjectsCreationRequest();
        private readonly CommandCreationRequest _CommandCreationRequest = new CommandCreationRequest();
        private readonly List<EntityDescriptor<int>> _ArraysBuildingRequested = new List<EntityDescriptor<int>>();
        private readonly List<IJSCSGlue> _BasicObjectsToCreate = new List<IJSCSGlue>();

        public JavascriptObjectBulkBuilder(IJavascriptObjectFactory factory, IJavascriptSessionCache cache, IBulkUpdater bulkPropertyUpdater, 
            IJSCSGlue root, bool needToCacheObject)
        {
            _NeedToCacheObject = needToCacheObject;
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
                _Cache.Cache(glueObject);
                return;
            }

            _BasicObjectsToCreate.Add(glueObject);
        }

        internal void RequestCommandCreation(IJSCSGlue glueObject, bool canExecute)
        {
            _CommandCreationRequest.AddRequest(glueObject, canExecute);
        }

        private void CreateObjects()
        {
            BulkCreate( () => _Factory.CreateBasics(_BasicObjectsToCreate.Select(glue => glue.CValue).ToList()), _BasicObjectsToCreate, false);
            BulkCreate( () => _Factory.CreateObjects(_ObjectsCreationRequest.ReadWriteNumber, _ObjectsCreationRequest.ReadOnlyNumber), _ObjectsCreationRequest.GetElements(), _NeedToCacheObject);
            BulkCreate( () => _Factory.CreateArrays(_ArraysBuildingRequested.Count), _ArraysBuildingRequested.Select(item => item.Father), _NeedToCacheObject);
            BulkCreate( () => _Factory.CreateComands(_CommandCreationRequest.CanExecuteNumber, _CommandCreationRequest.CanNotExecuteNumber), _CommandCreationRequest.GetElements(), _NeedToCacheObject);
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
            var toBeUpdated = _ArraysBuildingRequested.Where(item => item.ChildrenDescription.Length > 0);
            _BulkPropertyUpdater.BulkUpdateArray(toBeUpdated);
        }

        private void BulkCreate(Func<IEnumerable<IJavascriptObject>> builder, IEnumerable<IJSCSGlue> glues, bool register)
        {
            var objects = builder();

            if (register)
            {
                glues.ZipForEach(objects, (glue , @object) =>
                {
                    glue.SetJSValue(@object);
                    _Cache.Cache(glue);
                });
                return;
            }

            glues.ZipForEach(objects, (glue, @object) =>
            {
                glue.SetJSValue(@object);
            });
        }
    }
}
