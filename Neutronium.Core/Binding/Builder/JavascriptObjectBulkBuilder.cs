using MoreCollection.Extensions;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Infra.Reflection;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System.Collections.Generic;
using System.Linq;

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
        private readonly List<IJsCsGlue> _BasicObjectsToCreate = new List<IJsCsGlue>();

        private List<IJsCsGlue> _ExecutableObjectsToCreate = new List<IJsCsGlue>();
        private List<IJsCsGlue> ExecutableObjectsToCreate => _ExecutableObjectsToCreate ?? (_ExecutableObjectsToCreate = new List<IJsCsGlue>());

        private List<ArrayDescriptor> _ArraysBuildingRequested;
        private List<ArrayDescriptor> ArraysBuildingRequested => _ArraysBuildingRequested ?? (_ArraysBuildingRequested = new List<ArrayDescriptor>());

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
            var allBuilder = new JsAllBuilderAdapter(this);
            _Root.VisitDescendantsSafe(allBuilder.Visit);

            CreateObjects();
            UpdateDependencies();
        }

        internal void RequestObjectCreation(IJsCsGlue glue, IGenericPropertyAcessor attributeDescription, IReadOnlyList<IJsCsGlue> attributeValue)
        {
            _ObjectsCreationRequest.AddRequest(EntityDescriptor.CreateObjectDescriptor(glue, attributeDescription, attributeValue), attributeDescription.Observability);
        }

        internal void RequestArrayCreation(IJsCsGlue glue, IReadOnlyList<IJsCsGlue> children)
        {
            ArraysBuildingRequested.Add(EntityDescriptor.CreateArrayDescriptor(glue, children));
        }

        internal void RequestBasicObjectCreation(IJsCsGlue glueObject)
        {
            _BasicObjectsToCreate.Add(glueObject);
        }

        internal void RequestCommandCreation(IJsCsGlue glueObject, bool canExecute)
        {
            if (!_Mapping)
            {
                _CommandCreationRequest.AddRequest(glueObject, canExecute);
                return;
            }
            var command = _Factory.CreateObject(ObjectObservability.ReadOnlyObservable);
            command.SetValue("CanExecuteValue", _Factory.CreateBool(canExecute));
            command.SetValue("CanExecuteCount", _Factory.CreateInt(1));
            glueObject.SetJsValue(command, _Cache);
        }

        internal void RequestExecutableCreation(IJsCsGlue glueObject)
        {
            if (!_Mapping)
            {
                ExecutableObjectsToCreate.Add(glueObject);
                return;
            }

            var command = _Factory.CreateObject(ObjectObservability.ReadOnlyObservable);
            glueObject.SetJsValue(command, _Cache);
        }

        private void CreateObjects()
        {
            BulkCreate(_Factory.CreateFromExcecutionCode(_BasicObjectsToCreate.Select(glue => ((IBasicJsCsGlue)glue).GetCreationCode())), _BasicObjectsToCreate);
            BulkCreateObjects();
            BulkCreateArrays();

            if (_Mapping)
                return;

            BulkCreateCommands();
            BulkCreateExecutableObjects();
        }

        private void BulkCreateObjects()
        {
            var jsObjects = _Factory.CreateObjects(_ObjectsCreationRequest.ObjectsCreationOption);
            _ObjectsCreationRequest.SetJsObjects(jsObjects, SetJsValue);
        }

        private void BulkCreateArrays()
        {
            if (_ArraysBuildingRequested == null)
                return;

            var jsObjects = _Factory.CreateArrays(_ArraysBuildingRequested.Count);
            _ArraysBuildingRequested.ZipForEach(jsObjects, (desc, js) => desc.Father.SetJsValue(js, _Cache));
        }

        private void BulkCreateCommands()
        {
            BulkCreateCommand(_CommandCreationRequest.CommandExecutableBuildingRequested, true);
            BulkCreateCommand(_CommandCreationRequest.CommandNotExecutableBuildingRequested, false);
        }

        private void BulkCreateCommand(IList<IJsCsGlue> commands, bool canExecute)
        {
            if (commands == null)
                return;

            BulkCreate(_Factory.CreateObjectsFromConstructor(commands.Count, _BulkUpdater.CommandConstructor, _Factory.CreateBool(canExecute)), commands);
        }

        private void BulkCreateExecutableObjects()
        {
            if (_ExecutableObjectsToCreate == null)
                return;

            BulkCreate(_Factory.CreateObjectsFromConstructor(_ExecutableObjectsToCreate.Count, _BulkUpdater.ExecutableConstructor), _ExecutableObjectsToCreate);
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
            if (_ArraysBuildingRequested == null)
                return;

            var toBeUpdated = _ArraysBuildingRequested.Where(item => item.OrdenedChildren.Count > 0);
            _BulkUpdater.BulkUpdateArray(toBeUpdated);
        }

        private void BulkCreate(IEnumerable<IJavascriptObject> objects, IEnumerable<IJsCsGlue> glues)
        {
            glues.ZipForEach(objects, SetJsValue);
        }

        private void SetJsValue(IJsCsGlue glue, IJavascriptObject @object)
        {
            glue.SetJsValue(@object, _Cache);
        }
    }
}
