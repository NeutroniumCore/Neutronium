using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;
using System.Linq;

namespace Neutronium.Core.Binding.Builder
{
    internal class ObjectsCreationRequest
    {
        public int ReadWriteNumber => _ObjectReadWriteBuildingRequested.Count;
        public int ReadOnlyNumber => _ObjectReadOnlyBuildingRequested.Count;

        private readonly List<ObjectDescriptor> _ObjectReadWriteBuildingRequested = new List<ObjectDescriptor>();
        private readonly List<ObjectDescriptor> _ObjectReadOnlyBuildingRequested = new List<ObjectDescriptor>();

        public void AddRequest(ObjectDescriptor descriptor, bool updatableFromJs)
        {
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
            return _ObjectReadWriteBuildingRequested.Concat(_ObjectReadOnlyBuildingRequested).Where(item => item.ChildrenDescription.Count > 0);
        }
    }
}
