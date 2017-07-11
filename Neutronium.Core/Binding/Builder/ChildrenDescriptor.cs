using Neutronium.Core.Binding.GlueObject;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Binding.Builder
{
    public struct ChildDescription<TIdentificator>
    {
        public TIdentificator Key { get; }
        public IJSCSGlue Child { get; }

        public ChildDescription(TIdentificator key, IJSCSGlue child)
        {
            Key = key;
            Child = child;
        }
    }

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

    public class ChildrenPropertyDescriptor: ChildrenDescriptor<string>
    {
        public ChildrenPropertyDescriptor(IJSCSGlue father, IReadOnlyDictionary<string, IJSCSGlue> description) : 
            base(father, description?.Select(d => new ChildDescription<string>(d.Key, d.Value)))
        {
        }
    }

    public class ChildrenArrayDescriptor : ChildrenDescriptor<int>
    {
        public ChildrenArrayDescriptor(IJSCSGlue father, IList<IJSCSGlue> description) :
            base(father, description?.Select( (d, index) => new ChildDescription<int>(index, d)))
        {
        }
    }
}