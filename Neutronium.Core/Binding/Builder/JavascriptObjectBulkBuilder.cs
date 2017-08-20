using MoreCollection.Extensions;
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
        private readonly IBulkUpdater _BulkUpdater;
        private readonly IJSCSGlue _Root;
        private readonly bool _Mapping;

        private readonly ObjectsCreationRequest _ObjectsCreationRequest = new ObjectsCreationRequest();
        private readonly CommandCreationRequest _CommandCreationRequest = new CommandCreationRequest();
        private readonly List<EntityDescriptor<int>> _ArraysBuildingRequested = new List<EntityDescriptor<int>>();
        private readonly List<IJSCSGlue> _BasicObjectsToCreate = new List<IJSCSGlue>();
        private readonly List<IJSCSGlue> _ExecutableObjectsToCreate = new List<IJSCSGlue>();

        public JavascriptObjectBulkBuilder(IJavascriptObjectFactory factory, IJavascriptSessionCache cache, IBulkUpdater bulkPropertyUpdater, 
            IJSCSGlue root, bool mapping)
        {
            _Mapping = mapping;
            _Factory = factory;
            _Cache = cache;
            _Root = root;
            _BulkUpdater = bulkPropertyUpdater;
        }

        public void UpdateJavascriptValue()
        {
            var allBuilder = new JSAllBuilderAdapter(this);
            _Root.VisitAllChildren(allBuilder.Visit);

            CreateObjects();
            UpdateDependencies();
        }

        internal void RequestObjectCreation(IJSCSGlue glue, IEnumerable<KeyValuePair<string, IJSCSGlue>> children, bool updatableFromJS)
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
            if (!_Mapping)
            {
                _CommandCreationRequest.AddRequest(glueObject, canExecute);
                return;
            }

            var command = _Factory.CreateObject(true);
            command.SetValue("CanExecuteValue", _Factory.CreateBool(canExecute));
            command.SetValue("CanExecuteCount", _Factory.CreateInt(1));
            glueObject.SetJSValue(command);
        }

        internal void RequestExecutableCreation(IJSCSGlue glueObject)
        {
            if (!_Mapping)
            {
                _ExecutableObjectsToCreate.Add(glueObject);
                return;
            }

            var command = _Factory.CreateObject(true);
            glueObject.SetJSValue(command);
        }

        private void CreateObjects()
        {
            BulkCreate( () => _Factory.CreateBasics(_BasicObjectsToCreate.Select(glue => glue.CValue)), _BasicObjectsToCreate, false);
            BulkCreate( () => _Factory.CreateObjects(_ObjectsCreationRequest.ReadWriteNumber, _ObjectsCreationRequest.ReadOnlyNumber), _ObjectsCreationRequest.GetElements(), !_Mapping);
            BulkCreate( () => _Factory.CreateArrays(_ArraysBuildingRequested.Count), _ArraysBuildingRequested.Select(item => item.Father), !_Mapping);

            if (_Mapping)
                return;

            BulkCreateCommand( _CommandCreationRequest.CommandExecutableBuildingRequested, true);
            BulkCreateCommand( _CommandCreationRequest.CommandNotExecutableBuildingRequested, false);
            BulkCreate(() => _Factory.CreateObjectsFromContructor(_ExecutableObjectsToCreate.Count, _BulkUpdater.ExecutableConstructor), _ExecutableObjectsToCreate, true);
        }

        private void BulkCreateCommand(IList<IJSCSGlue> commands, bool canExecute)
        {
            BulkCreate(() => _Factory.CreateObjectsFromContructor(commands.Count, _BulkUpdater.CommandConstructor, _Factory.CreateBool(canExecute)), commands, true);
        }

        private void UpdateDependencies()
        {
            UpdateObjects();
            UpdateArrays();  
        }

        private void UpdateObjects()
        {
            var toBeUpdated = _ObjectsCreationRequest.GetElementWithProperty();          
            _BulkUpdater.BulkUpdateProperty(toBeUpdated);
        }

        private void UpdateArrays()
        {
            var toBeUpdated = _ArraysBuildingRequested.Where(item => item.ChildrenDescription.Length > 0);
            _BulkUpdater.BulkUpdateArray(toBeUpdated);
        }

        private void BulkCreate(Func<IEnumerable<IJavascriptObject>> builder, IEnumerable<IJSCSGlue> glues, bool register)
        {
            var objects = builder();

            if (!register)
            {
                glues.ZipForEach(objects, (glue, @object) => glue.SetJSValue(@object));
                return;
            }

            glues.ZipForEach(objects, (glue, @object) =>
            {
                glue.SetJSValue(@object);
                _Cache.Cache(glue);
            });
        }
    }
}
