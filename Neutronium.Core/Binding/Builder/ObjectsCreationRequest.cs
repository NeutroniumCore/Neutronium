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
            new ObjectsCreationOption(_ObjectReadWriteBuildingRequested.Count, _ObjectReadOnlyBuildingRequested.Count);

        private readonly List<ObjectDescriptor> _ObjectReadWriteBuildingRequested = new List<ObjectDescriptor>();
        private readonly List<ObjectDescriptor> _ObjectReadOnlyBuildingRequested = new List<ObjectDescriptor>();

        public void AddRequest(ObjectDescriptor descriptor, ObjectObservability objectObservability)
        {
            var updatableFromJs = !objectObservability.HasFlag(ObjectObservability.ReadOnly);
            if (!updatableFromJs)
            {
                _ObjectReadOnlyBuildingRequested.Add(descriptor);
                return;
            }

            _ObjectReadWriteBuildingRequested.Add(descriptor);       
        }

        internal IEnumerable<IJsCsGlue> GetElements()
        {
            return _ObjectReadWriteBuildingRequested.Concat(_ObjectReadOnlyBuildingRequested).Select(item => item.Father);
        }

        internal IEnumerable<ObjectDescriptor> GetElementWithProperty()
        {
            return _ObjectReadWriteBuildingRequested.Concat(_ObjectReadOnlyBuildingRequested).Where(item => item.AttributeValues.Count > 0);
        }
    }
}
