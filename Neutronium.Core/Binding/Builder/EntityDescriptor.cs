using Neutronium.Core.Binding.GlueObject;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Binding.Builder
{
    public struct ObjectDescriptor
    {
        public IJSCSGlue Father { get; }
        public ICollection<KeyValuePair<string, IJSCSGlue>> ChildrenDescription { get; }

        public ObjectDescriptor(IJSCSGlue father, ICollection<KeyValuePair<string, IJSCSGlue>> childrenDescription)
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
        public IJSCSGlue Father { get; }
        public IList<IJSCSGlue> OrdenedChildren { get; }
        public int OffSet { get; }

        public ArrayDescriptor(IJSCSGlue father, IList<IJSCSGlue> childrenDescription, int offfset=0)
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
        public static ArrayDescriptor CreateArrayDescriptor(IJSCSGlue father, IList<IJSCSGlue> description) 
        {
            return new ArrayDescriptor(father, description);
        }

        public static ObjectDescriptor CreateObjectDescriptor(IJSCSGlue father, ICollection<KeyValuePair<string, IJSCSGlue>> description)
        {
            return new ObjectDescriptor(father, description);
        }
    }
}