using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;
using System.Linq;

namespace Neutronium.Core.Binding.Builder
{
    internal class ObjectsCreationRequest
    {       
        public int ReadWriteNumber { get; set; }
        public int ReadOnlyNumber => _ObjectBuildingRequested.Count - ReadWriteNumber;

        private readonly LinkedList<EntityDescriptor<string>> _ObjectBuildingRequested = new LinkedList<EntityDescriptor<string>>();

        public void AddRequest(EntityDescriptor<string> descriptor, bool updatableFromJS)
        {
            if (!updatableFromJS)
            {
                _ObjectBuildingRequested.AddLast(descriptor);
                return;
            }

            ReadWriteNumber += 1;
            _ObjectBuildingRequested.AddFirst(descriptor);       
        }

        internal IEnumerable<IJSCSGlue> GetElements()
        {
            return _ObjectBuildingRequested.Select(item => item.Father);
        }

        internal IEnumerable<EntityDescriptor<string>> GetElementWithProperty()
        {
            return _ObjectBuildingRequested.Where(item => item.ChildrenDescription.Length > 0);
        }
    }
}
