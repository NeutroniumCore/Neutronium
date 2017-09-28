using System;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.Builder.Packer
{
    internal class ObjectChildrenDescriptionPacker
    {
        public string Pack(List<ObjectDescriptor> updates)
        {
            return KeyPacker.Pack(PackKeys(updates));
        }

        private static IEnumerable<Tuple<int, IEnumerable<string>>> PackKeys(List<ObjectDescriptor> updates)
        {
            var count = 1;
            ObjectDescriptor? currentDescriptor = null;
            foreach (var update in updates)
            {
                if (!currentDescriptor.HasValue)
                {
                    currentDescriptor = update;
                    continue;
                }
                
                if (ReferenceEquals(update.AttributeNames, currentDescriptor.Value.AttributeNames))
                {
                    count++;
                    continue;
                }

                yield return Tuple.Create(count, currentDescriptor.Value.AttributeNames);
                currentDescriptor = update;
                count = 1;
            }

            if (currentDescriptor.HasValue)
                yield return Tuple.Create(count, currentDescriptor.Value.AttributeNames);
        }
    }
}
