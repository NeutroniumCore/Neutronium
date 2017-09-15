using MoreCollection.Extensions;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Extension;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;
using System.Collections.Generic;
using System.Linq;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.Binding.Builder
{
    internal class JavascriptObjectBulkBuilder
    {
        private readonly IJavascriptObjectFactory _Factory;
        private readonly IJavascriptSessionCache _Cache;
        private readonly IBulkUpdater _BulkUpdater;
        private readonly IJsCsGlue _Root;
        private readonly bool _Mapping;

        private readonly ObjectsCreationRequest _ObjectsCreationRequest = new ObjectsCreationRequest();
        private readonly CommandCreationRequest _CommandCreationRequest = new CommandCreationRequest();
        private readonly List<ArrayDescriptor> _ArraysBuildingRequested = new List<ArrayDescriptor>();
        private readonly List<IJsCsGlue> _BasicObjectsToCreate = new List<IJsCsGlue>();
        private readonly List<IJsCsGlue> _ExecutableObjectsToCreate = new List<IJsCsGlue>();

        public JavascriptObjectBulkBuilder(IJavascriptObjectFactory factory, IJavascriptSessionCache cache, IBulkUpdater bulkPropertyUpdater, 
            IJsCsGlue root, bool mapping)
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
            _Root.VisitDescendants(allBuilder.Visit);

            CreateObjects();
            UpdateDependencies();
        }

        internal void RequestObjectCreation(IJsCsGlue glue, TypePropertyAccessor attributeDescription, IJsCsGlue[] attributeValue)
        {
            var updatableFromJs = attributeDescription.HasReadWriteProperties;
            _ObjectsCreationRequest.AddRequest(EntityDescriptor.CreateObjectDescriptor(glue, attributeDescription, attributeValue), updatableFromJs);
        }

        internal void RequestArrayCreation(IJsCsGlue glue, IList<IJsCsGlue> children)
        {
            _ArraysBuildingRequested.Add(EntityDescriptor.CreateArrayDescriptor(glue, children));
        }

        internal void RequestBasicObjectCreation(IJsCsGlue glueObject, object cValue)
        {
            if (cValue == null)
            {
                glueObject.SetJsValue(_Factory.CreateNull());
                return;
            }

            if (cValue.GetType().IsEnum)
            {
                glueObject.SetJsValue(_Factory.CreateEnum((Enum)cValue));
                _Cache.Cache(glueObject);
                return;
            }

            _BasicObjectsToCreate.Add(glueObject);
        }

        internal void RequestCommandCreation(IJsCsGlue glueObject, bool canExecute)
        {
            if (!_Mapping)
            {
                _CommandCreationRequest.AddRequest(glueObject, canExecute);
                return;
            }


            var command = _Factory.CreateObject(true);
            command.SetValue("CanExecuteValue", _Factory.CreateBool(canExecute));
            command.SetValue("CanExecuteCount", _Factory.CreateInt(1));
            glueObject.SetJsValue(command);
        }

        internal void RequestExecutableCreation(IJsCsGlue glueObject)
        {
            if (!_Mapping)
            {
                _ExecutableObjectsToCreate.Add(glueObject);
                return;
            }

            var command = _Factory.CreateObject(true);
            glueObject.SetJsValue(command);
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

        private void BulkCreateCommand(IList<IJsCsGlue> commands, bool canExecute)
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
            var toBeUpdated = _ArraysBuildingRequested.Where(item => item.OrdenedChildren.Count > 0);
            _BulkUpdater.BulkUpdateArray(toBeUpdated);
        }

        private void BulkCreate(Func<IEnumerable<IJavascriptObject>> builder, IEnumerable<IJsCsGlue> glues, bool register)
        {
            var objects = builder();

            if (!register)
            {
                glues.ZipForEach(objects, (glue, @object) => glue.SetJsValue(@object));
                return;
            }

            glues.ZipForEach(objects, (glue, @object) =>
            {
                glue.SetJsValue(@object);
                _Cache.Cache(glue);
            });
        }
    }
}
