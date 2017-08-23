using System;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Binding.Builder.Packer
{
    internal class ObjectChildrenDescriptionPacker : IEntityDescriptorChildrenDescriptionPacker<string>
    {
        public string Pack(List<EntityDescriptor<string>> updates)
        {
            return KeyPacker.Pack(updates.Count, PackKeys(updates), s => $@"""{s}""");
        }

        private struct keyDescription
        {
            public EntityDescriptor<string> Descritor { get; set; }
            public Type Type { get; set; }
            public int Count { get; set; }

            public keyDescription(EntityDescriptor<string> update)
            {
                Descritor = update;
                Type = update.Father.CValue.GetType();
                Count = update.ChildrenDescription.Count;
            }

            public bool Similar(keyDescription other)
            {
                return Count == other.Count && Type == other.Type;
            }

            public List<string> Keys => Descritor.ChildrenDescription.Select(k => k.Key).ToList();

            public bool Empty => Type == null;
        }

        private static IEnumerable<Tuple<int, List<string>>> PackKeys(List<EntityDescriptor<string>> updates)
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
