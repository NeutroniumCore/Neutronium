using System;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Binding.Builder.Packer
{
    internal class ArrayChildrenDescriptionPacker
    {
        public string Pack(List<ArrayDescriptor> updates)
        {
            return KeyPacker.Pack(updates.Count, PackKeys(updates), t => t.ToString());
        }

        private struct KeyDescription
        {
            public ArrayDescriptor Descritor { get; }
            public int Start { get; }
            public int Count { get; }

            public KeyDescription(ArrayDescriptor update)
            {
                Descritor = update;
                Start = update.OffSet;
                Count = update.OrdenedChildren.Count;
            }

            public bool Similar(KeyDescription other)
            {
                return Count == other.Count && Start == other.Start;
            }

            public List<int> Keys => Enumerable.Range(Start, Count).ToList();

            public bool Empty => Count == 0;
        }

        private static IEnumerable<Tuple<int, List<int>>> PackKeys(List<ArrayDescriptor> updates)
        {
            var count = 0;
            var first = true;
            var currentDescriptor = new KeyDescription();
            foreach (var update in updates)
            {
                var newDesc = new KeyDescription(update);
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
