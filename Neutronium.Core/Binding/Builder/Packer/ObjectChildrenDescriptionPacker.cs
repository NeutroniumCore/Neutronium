using System;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.Builder.Packer
{
    internal class ObjectChildrenDescriptionPacker
    {
        public string Pack(List<ObjectDescriptor> updates)
        {
            return KeyPacker.Pack(updates.Count, PackKeys(updates), s => $@"""{s}""");
        }

        private struct KeyDescription
        {
            private ObjectDescriptor Descritor { get; }
            private int Count { get; }

            public KeyDescription(ObjectDescriptor update)
            {
                Descritor = update;
                Count = update.AttributeValues.Length;
            }

            public bool Similar(KeyDescription other)
            {
                return Count == other.Count && ReferenceEquals(Descritor.AttributeNames, other.Descritor.AttributeNames);
            }

            public string[] Keys => Descritor.AttributeNames;

            public bool Empty => Descritor.Father == null;
        }

        private static IEnumerable<Tuple<int, string[]>> PackKeys(List<ObjectDescriptor> updates)
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
