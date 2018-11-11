using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Infra;
using Neutronium.Core.Infra.Reflection;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public void AddRequest(ObjectDescriptor descriptor, ObjectObservability objectObservability)
        {
            var list = GetList(objectObservability);
            list.Add(descriptor);
        }

        internal IEnumerable<ObjectDescriptor> GetElementWithProperty()
        {
            var descriptors = _ObjectNoneBuildingRequested
                .SafeConcat(_ObjectReadOnlyBuildingRequested)
                .SafeConcat(_ObjectObservableBuildingRequested)
                .SafeConcat(_ObjectObservableReadOnlyBuildingRequested);
            return descriptors.Where(item => item.AttributeValues.Count > 0);
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

        public void SetJsObjects(IEnumerable<IJavascriptObject> jsObjects, Action<IJsCsGlue, IJavascriptObject> onJsValue)
        {
            using (var enumerator = jsObjects.GetEnumerator())
            {
                void SetJsValue(ObjectDescriptor glue)
                {
                    enumerator.MoveNext();
                    onJsValue(glue.Father, enumerator.Current);
                }

                _ObjectNoneBuildingRequested?.ForEach(SetJsValue);
                _ObjectReadOnlyBuildingRequested?.ForEach(SetJsValue);
                _ObjectObservableBuildingRequested?.ForEach(SetJsValue);
                _ObjectObservableReadOnlyBuildingRequested?.ForEach(SetJsValue);
            }
        }
    }
}
