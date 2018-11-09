using MoreCollection.Extensions;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
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
            var allBuilder = new JsAllBuilderAdapter(this);
            _Root.VisitDescendantsSafe(allBuilder.Visit);

            _ObjectsCreationRequest.Freeze();

            CreateObjects();
            UpdateDependencies();
        }

        internal void RequestObjectCreation(IJsCsGlue glue, IGenericPropertyAcessor attributeDescription, IReadOnlyList<IJsCsGlue> attributeValue)
        {
            _ObjectsCreationRequest.AddRequest(EntityDescriptor.CreateObjectDescriptor(glue, attributeDescription, attributeValue), attributeDescription.Observability);
        }

        internal void RequestArrayCreation(IJsCsGlue glue, IReadOnlyList<IJsCsGlue> children)
        {
            _ArraysBuildingRequested.Add(EntityDescriptor.CreateArrayDescriptor(glue, children));
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
                _ExecutableObjectsToCreate.Add(glueObject);
                return;
            }

            var command = _Factory.CreateObject(ObjectObservability.ReadOnlyObservable);
            glueObject.SetJsValue(command, _Cache);
        }

        private void CreateObjects()
        {
            BulkCreate( _Factory.CreateFromExcecutionCode(_BasicObjectsToCreate.Select(glue => ((IBasicJsCsGlue)glue).GetCreationCode())), _BasicObjectsToCreate);
            BulkCreate( _Factory.CreateObjects(_ObjectsCreationRequest.ObjectsCreationOption), _ObjectsCreationRequest.GetElements());
            BulkCreate( _Factory.CreateArrays(_ArraysBuildingRequested.Count), _ArraysBuildingRequested.Select(item => item.Father));

            if (_Mapping)
                return;

            BulkCreateCommand( _CommandCreationRequest.CommandExecutableBuildingRequested, true);
            BulkCreateCommand( _CommandCreationRequest.CommandNotExecutableBuildingRequested, false);
            BulkCreate(_Factory.CreateObjectsFromConstructor(_ExecutableObjectsToCreate.Count, _BulkUpdater.ExecutableConstructor), _ExecutableObjectsToCreate);
        }

        private void BulkCreateCommand(IList<IJsCsGlue> commands, bool canExecute)
        {
            BulkCreate(_Factory.CreateObjectsFromConstructor(commands.Count, _BulkUpdater.CommandConstructor, _Factory.CreateBool(canExecute)), commands);
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

        private void BulkCreate(IEnumerable<IJavascriptObject> objects, IEnumerable<IJsCsGlue> glues)
        {
            glues.ZipForEach(objects, (glue, @object) => glue.SetJsValue(@object, _Cache));
        }
    }
}
