using System;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.Builder.Packer
{
    internal class ArrayChildrenDescriptionPacker
    {
        public string Pack(List<ArrayDescriptor> updates)
        {
            return KeyPacker.Pack(PackKeys(updates));
        }

        private static IEnumerable<Tuple<int, ArrayDescriptor>> PackKeys(IList<ArrayDescriptor> updates)
        {
            var count = 1;
            ArrayDescriptor? arrayDescriptor= null;
            foreach (var update in updates)
            {
                if (!arrayDescriptor.HasValue)
                {
                    arrayDescriptor = update;
                    continue;
                }

                if ((update.OffSet == arrayDescriptor.Value.OffSet) && (update.Count == arrayDescriptor.Value.Count))
                {
                    count++;
                    continue;
                }

                yield return Tuple.Create(count, arrayDescriptor.Value);
                arrayDescriptor = update;
                count = 1;
            }

            if (arrayDescriptor.HasValue)
                yield return Tuple.Create(count, arrayDescriptor.Value);
        }
    }
}
