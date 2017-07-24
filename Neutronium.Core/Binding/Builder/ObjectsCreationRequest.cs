using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;
using System.Linq;

namespace Neutronium.Core.Binding.Builder
{
    internal class ObjectsCreationRequest
    {
        public int ReadOnlyNumber => _ObjectsReadOnlyBuildingRequested.Count;
        public int ReadWriteNumber => _ObjectsReadWriteBuildingRequested.Count;

        private readonly List<EntityDescriptor<string>> _ObjectsReadOnlyBuildingRequested = new List<EntityDescriptor<string>>();
        private readonly List<EntityDescriptor<string>> _ObjectsReadWriteBuildingRequested = new List<EntityDescriptor<string>>();

        private IEnumerable<EntityDescriptor<string>> AllElements => _ObjectsReadWriteBuildingRequested.Concat(_ObjectsReadOnlyBuildingRequested);

        public void AddRequest(EntityDescriptor<string> descriptor, bool updatableFromJS)
        {
            if (updatableFromJS)
                _ObjectsReadWriteBuildingRequested.Add(descriptor);
            else
                _ObjectsReadOnlyBuildingRequested.Add(descriptor);
        }

        internal List<IJSCSGlue> GetElements()
        {
            return AllElements.Select(item => item.Father).ToList();
        }

        internal List<EntityDescriptor<string>> GetElementWithProperty()
        {
            return AllElements.Where(item => item.ChildrenDescription.Length > 0).ToList();
        }
    }
}
