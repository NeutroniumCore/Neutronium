using System;
using System.Collections.Generic;
using System.Linq;

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
            private Type Type { get; }
            private int Count { get; }

            public KeyDescription(ObjectDescriptor update)
            {
                Descritor = update;
                Type = update.Father.CValue.GetType();
                Count = update.ChildrenDescription.Count;
            }

            public bool Similar(KeyDescription other)
            {
                return Count == other.Count && Type == other.Type;
            }

            public List<string> Keys => Descritor.ChildrenDescription.Select(k => k.Key).ToList();

            public bool Empty => Type == null;
        }

        private static IEnumerable<Tuple<int, List<string>>> PackKeys(List<ObjectDescriptor> updates)
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
