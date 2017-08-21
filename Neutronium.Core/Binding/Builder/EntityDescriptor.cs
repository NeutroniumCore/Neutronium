using Neutronium.Core.Binding.GlueObject;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Binding.Builder
{
    public struct EntityDescriptor<TIdentificator>
    {
        public IJSCSGlue Father { get; }
        public ICollection<KeyValuePair<TIdentificator, IJSCSGlue>> ChildrenDescription { get; }

        public EntityDescriptor(IJSCSGlue father, ICollection<KeyValuePair<TIdentificator, IJSCSGlue>> childrenDescription)
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
            if (!(obj is EntityDescriptor<TIdentificator>))
                return false;
            var other = (EntityDescriptor<TIdentificator>)obj;
            return (Father == other.Father) && ChildrenDescription.SequenceEqual(other.ChildrenDescription);
        }
    }

    public static class EntityDescriptor
    {
        public static EntityDescriptor<int> CreateArrayDescriptor(IJSCSGlue father, IList<IJSCSGlue> description)
        {
            return new EntityDescriptor<int>(father, description?.Select((d, index) => new KeyValuePair<int, IJSCSGlue>(index, d)).ToArray());
        }

        public static EntityDescriptor<string> CreateObjectDescriptor(IJSCSGlue father, ICollection<KeyValuePair<string, IJSCSGlue>> description)
        {
            return new EntityDescriptor<string>(father, description);
        }
    }
}