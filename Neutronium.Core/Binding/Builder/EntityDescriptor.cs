using Neutronium.Core.Binding.GlueObject;
using System.Collections.Generic;
using System.Linq;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.Binding.Builder
{
    public struct ObjectDescriptor
    {
        public IJsCsGlue Father { get; }
        public IEnumerable<string> AttributeNames { get; }
        public IReadOnlyList<IJsCsGlue> AttributeValues { get; }

        internal ObjectDescriptor(IJsCsGlue father, IEnumerable<string> attributes, IReadOnlyList<IJsCsGlue> attributeValues)
        {
            Father = father;
            AttributeNames = attributes;
            AttributeValues = attributeValues;
        }

        internal ObjectDescriptor Take(int take) => new ObjectDescriptor(Father, AttributeNames.Take(take).ToArray(), AttributeValues.Take(take).ToArray());

        internal ObjectDescriptor Split(int start, int take) => new ObjectDescriptor(Father, AttributeNames.Skip(start).Take(take), AttributeValues.Skip(start).Take(take).ToArray());

        internal ObjectDescriptor Skip(int start) => new ObjectDescriptor(Father, AttributeNames.Skip(start), AttributeValues.Skip(start).ToArray());

        public override int GetHashCode()
        {
            return Father?.GetHashCode() ?? 0;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ObjectDescriptor))
                return false;
            var other = (ObjectDescriptor)obj;
            return (Father == other.Father) && AttributeValues.SequenceEqual(other.AttributeValues);
        }
    }

    internal struct ArrayDescriptor
    {
        public IJsCsGlue Father { get; }
        public IReadOnlyList<IJsCsGlue> OrdenedChildren { get; }
        public int OffSet { get; }
        public int Count { get; }

        internal ArrayDescriptor(IJsCsGlue father, IReadOnlyList<IJsCsGlue> childrenDescription, int offfset = 0)
        {
            Father = father;
            OrdenedChildren = childrenDescription;
            OffSet = offfset;
            Count = childrenDescription.Count;
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

    internal static class EntityDescriptor
    {
        internal static ArrayDescriptor CreateArrayDescriptor(IJsCsGlue father, IReadOnlyList<IJsCsGlue> description)
        {
            return new ArrayDescriptor(father, description);
        }

        internal static ObjectDescriptor CreateObjectDescriptor(IJsCsGlue father, IGenericPropertyAcessor attributeDescription, IReadOnlyList<IJsCsGlue> attributeValue)
        {
            return new ObjectDescriptor(father, attributeDescription.AttributeNames, attributeValue);
        }
    }
}