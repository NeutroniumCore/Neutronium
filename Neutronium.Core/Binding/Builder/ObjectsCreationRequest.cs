using System;
using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;
using System.Linq;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.Binding.Builder
{
    internal class ObjectsCreationRequest
    {
        public ObjectsCreationOption ObjectsCreationOption =>
            new ObjectsCreationOption(
                _ObjectNoneBuildingRequested.Count,
                _ObjectReadOnlyBuildingRequested.Count,
                _ObjectObservableBuildingRequested.Count,
                _ObjectObservableReadOnlyBuildingRequested.Count);

        private readonly List<ObjectDescriptor> _ObjectNoneBuildingRequested = new List<ObjectDescriptor>();
        private readonly List<ObjectDescriptor> _ObjectReadOnlyBuildingRequested = new List<ObjectDescriptor>();
        private readonly List<ObjectDescriptor> _ObjectObservableBuildingRequested = new List<ObjectDescriptor>();
        private readonly List<ObjectDescriptor> _ObjectObservableReadOnlyBuildingRequested = new List<ObjectDescriptor>();

        private IEnumerable<ObjectDescriptor> AllObjectDescriptor => _ObjectNoneBuildingRequested
            .Concat(_ObjectReadOnlyBuildingRequested)
            .Concat(_ObjectObservableBuildingRequested)
            .Concat(_ObjectObservableReadOnlyBuildingRequested);

        public void AddRequest(ObjectDescriptor descriptor, ObjectObservability objectObservability)
        {
            var list = GetList(objectObservability);
            list.Add(descriptor);       
        }

        internal IEnumerable<IJsCsGlue> GetElements()
        {
            return AllObjectDescriptor.Select(item => item.Father);
        }

        internal IEnumerable<ObjectDescriptor> GetElementWithProperty()
        {
            return AllObjectDescriptor.Where(item => item.AttributeValues.Count > 0);
        }

        private List<ObjectDescriptor> GetList(ObjectObservability objectObservability)
        {
            switch (objectObservability)
            {
                case ObjectObservability.None:
                    return _ObjectNoneBuildingRequested;

                case ObjectObservability.ReadOnly:
                    return _ObjectReadOnlyBuildingRequested;

                case ObjectObservability.Observable:
                    return _ObjectObservableBuildingRequested;

                case ObjectObservability.ReadOnlyObservable:
                    return _ObjectObservableReadOnlyBuildingRequested;

                default:
                    throw new ArgumentOutOfRangeException(nameof(objectObservability), objectObservability, null);
            }
        }
    }
}
