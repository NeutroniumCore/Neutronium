using Neutronium.Core.Binding.GlueObject;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Binding.Builder
{
    public struct EntityDescriptor<TIdentificator>
    {
        public IJSCSGlue Father { get; }
        public ChildDescription<TIdentificator>[] ChildrenDescription { get; }

        public EntityDescriptor(IJSCSGlue father, IEnumerable<ChildDescription<TIdentificator>> childrenDescription)
        {
            Father = father;
            ChildrenDescription = (childrenDescription==null) ? new ChildDescription<TIdentificator>[0] : childrenDescription.ToArray();
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
            return new EntityDescriptor<int>(father, description?.Select((d, index) => new ChildDescription<int>(index, d)));
        }

        public static EntityDescriptor<string> CreateObjectDescriptor(IJSCSGlue father, IEnumerable<KeyValuePair<string, IJSCSGlue>> description)
        {
            return new EntityDescriptor<string>(father, description?.Select(d => new ChildDescription<string>(d.Key, d.Value)));
        }
    }
}