using System;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Binding.Builder.Packer
{
    internal static class KeyPacker
    {
        public static string AsArray(IEnumerable<string> value) => $"[{string.Join(",", value)}]";

        public static string Pack<T>(IEnumerable<Tuple<int, List<T>>> updates, Func<T, string> valueExtractor)
        {
            return AsArray(updates.Select(pack => $@"{{""c"":{pack.Item1},""a"":{AsArray(pack.Item2.Select(valueExtractor))}}}"));
        }

        public static string Pack<T>(IEnumerable<Tuple<int, T[]>> updates, Func<T, string> valueExtractor)
        {
            return AsArray(updates.Select(pack => $@"{{""c"":{pack.Item1},""a"":{AsArray(pack.Item2.Select(valueExtractor))}}}"));
        }
    }
}
