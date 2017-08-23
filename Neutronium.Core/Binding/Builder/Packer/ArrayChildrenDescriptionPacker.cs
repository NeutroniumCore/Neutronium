using System;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Binding.Builder.Packer
{
    internal class ArrayChildrenDescriptionPacker : IEntityDescriptorChildrenDescriptionPacker<int>
    {
        public string Pack(List<EntityDescriptor<int>> updates)
        {
            return KeyPacker.Pack(updates.Count, PackKeys(updates), t => t.ToString());
        }

        private struct keyDescription
        {
            public EntityDescriptor<int> Descritor { get; set; }
            public int? Start { get; set; }
            public int Count { get; set; }

            public keyDescription(EntityDescriptor<int> update)
            {
                Descritor = update;
                Start = update.ChildrenDescription.First().Key;
                Count = update.ChildrenDescription.Count;
            }

            public bool Similar(keyDescription other)
            {
                return Count == other.Count && Start == other.Start;
            }

            public List<int> Keys => Descritor.ChildrenDescription.Select(k => k.Key).ToList();

            public bool Empty => !Start.HasValue;
        }

        private static IEnumerable<Tuple<int, List<int>>> PackKeys(List<EntityDescriptor<int>> updates)
        {
            var count = 0;
            var first = true;
            var currentDescriptor = new keyDescription();
            foreach (var update in updates)
            {
                var newDesc = new keyDescription(update);
                if (first)
                {
                    currentDescriptor = newDesc;
                    first = false;
                    continue;
                }

                if (newDesc.Similar(currentDescriptor))
                {
                    count++;
                    continue;
                }

                yield return Tuple.Create(count, currentDescriptor.Keys);
                currentDescriptor = newDesc;
                count = 0;
            }

            if (!currentDescriptor.Empty)
                yield return Tuple.Create(count, currentDescriptor.Keys);
        }
    }
}
