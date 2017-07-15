using Neutronium.Core.Binding.GlueObject;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Binding.Builder
{
    public class ChildrenDescriptor<TIdentificator>
    {
        public IJSCSGlue Father { get; }
        public IReadOnlyCollection<ChildDescription<TIdentificator>> ChildrenDescription { get; }

        public ChildrenDescriptor(IJSCSGlue father, IEnumerable<ChildDescription<TIdentificator>> childrenDescription)
        {
            Father = father;
            ChildrenDescription = (childrenDescription==null) ? new List<ChildDescription<TIdentificator>>() : new List<ChildDescription<TIdentificator>>(childrenDescription);
        }

        public override int GetHashCode()
        {
            return Father?.GetHashCode()?? 0;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ChildrenDescriptor<TIdentificator>;
            return (other == null) ? false : (Father == other.Father) && ChildrenDescription.SequenceEqual(other.ChildrenDescription);
        }
    }
}