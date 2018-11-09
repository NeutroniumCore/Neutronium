using System;
using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;
using System.Linq;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.Infra.Reflection;
using Neutronium.Core.Infra;

namespace Neutronium.Core.Binding.Builder
{
    internal class ObjectsCreationRequest
    {
        public ObjectsCreationOption ObjectsCreationOption =>
            new ObjectsCreationOption(
                ListCount(_ObjectNoneBuildingRequested),
                ListCount(_ObjectReadOnlyBuildingRequested),
                ListCount(_ObjectObservableBuildingRequested),
                ListCount(_ObjectObservableReadOnlyBuildingRequested));

        private List<ObjectDescriptor> _ObjectNoneBuildingRequested;
        private List<ObjectDescriptor> _ObjectReadOnlyBuildingRequested;
        private List<ObjectDescriptor> _ObjectObservableBuildingRequested;
        private List<ObjectDescriptor> _ObjectObservableReadOnlyBuildingRequested;

        private static int ListCount(List<ObjectDescriptor> list) => (list?.Count).GetValueOrDefault();

        private IEnumerable<ObjectDescriptor> _AllObjectDescriptor;

        public void Freeze() 
        {
            _AllObjectDescriptor = _ObjectNoneBuildingRequested
                .SafeConcat(_ObjectReadOnlyBuildingRequested)
                .SafeConcat(_ObjectObservableBuildingRequested)
                .SafeConcat(_ObjectObservableReadOnlyBuildingRequested);
        }

        public void AddRequest(ObjectDescriptor descriptor, ObjectObservability objectObservability)
        {
            var list = GetList(objectObservability);
            list.Add(descriptor);       
        }

        internal IEnumerable<IJsCsGlue> GetElements()
        {
            return _AllObjectDescriptor.Select(item => item.Father);
        }

        internal IEnumerable<ObjectDescriptor> GetElementWithProperty()
        {
            return _AllObjectDescriptor.Where(item => item.AttributeValues.Count > 0);
        }

        private List<ObjectDescriptor> GetList(ObjectObservability objectObservability)
        {
            switch (objectObservability)
            {
                case ObjectObservability.None:
                    return GetOrCreate(ref _ObjectNoneBuildingRequested);

                case ObjectObservability.ReadOnly:
                    return GetOrCreate(ref _ObjectReadOnlyBuildingRequested);

                case ObjectObservability.Observable:
                    return GetOrCreate(ref _ObjectObservableBuildingRequested);

                case ObjectObservability.ReadOnlyObservable:
                    return GetOrCreate(ref _ObjectObservableReadOnlyBuildingRequested);

                default:
                    throw new ArgumentOutOfRangeException(nameof(objectObservability), objectObservability, null);
            }
        }

        private static List<ObjectDescriptor> GetOrCreate(ref List<ObjectDescriptor> from) 
        {
            if (from == null)
                from = new List<ObjectDescriptor>();
            return from;
        }
    }
}
