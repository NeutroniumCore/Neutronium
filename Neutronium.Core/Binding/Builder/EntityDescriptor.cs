using Neutronium.Core.Binding.GlueObject;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Binding.Builder
{
    public struct ObjectDescriptor
    {
        public IJsCsGlue Father { get; }
        public AttributeDescription[] ChildrenDescription { get; }

        public ObjectDescriptor(IJsCsGlue father, AttributeDescription[] childrenDescription)
        {
            Father = father;
            ChildrenDescription = childrenDescription;
        }

        public override int GetHashCode()
        {
            return Father?.GetHashCode() ?? 0;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ObjectDescriptor))
                return false;
            var other = (ObjectDescriptor)obj;
            return (Father == other.Father) && ChildrenDescription.SequenceEqual(other.ChildrenDescription);
        }
    }

    public struct ArrayDescriptor 
    {
        public IJsCsGlue Father { get; }
        public IList<IJsCsGlue> OrdenedChildren { get; }
        public int OffSet { get; }

        public ArrayDescriptor(IJsCsGlue father, IList<IJsCsGlue> childrenDescription, int offfset=0)
        {
            Father = father;
            OrdenedChildren = childrenDescription;
            OffSet = offfset;
        }

        public override int GetHashCode()
        {
            return Father?.GetHashCode() ?? 0;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ArrayDescriptor))
                return false;
            var other = (ArrayDescriptor)obj;
            return (Father == other.Father) && OrdenedChildren.SequenceEqual(other.OrdenedChildren);
        }
    }

    public static class EntityDescriptor
    {
        public static ArrayDescriptor CreateArrayDescriptor(IJsCsGlue father, IList<IJsCsGlue> description) 
        {
            return new ArrayDescriptor(father, description);
        }

        public static ObjectDescriptor CreateObjectDescriptor(IJsCsGlue father, AttributeDescription[] description)
        {
            return new ObjectDescriptor(father, description);
        }
    }
}