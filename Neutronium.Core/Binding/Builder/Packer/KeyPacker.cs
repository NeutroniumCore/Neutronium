using System;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Binding.Builder.Packer
{
    internal static class KeyPacker
    {
        public static string AsArray(IEnumerable<string> value) => $"[{string.Join(",", value)}]";

        public static string Pack(IEnumerable<Tuple<int, ArrayDescriptor>> updates)
        {
            return AsArray(updates.Select(pack => $@"{{""c"":{pack.Item1},""a"":{{""b"":{pack.Item2.OffSet},""e"":{pack.Item2.OffSet + pack.Item2.Count}}}}}"));
        }

        public static string Pack(IEnumerable<Tuple<int, string[]>> updates)
        {
            return AsArray(updates.Select(pack => $@"{{""c"":{pack.Item1},""a"":{AsArray(pack.Item2.Select(s => $@"""{s}"""))}}}"));
        }
    }
}
