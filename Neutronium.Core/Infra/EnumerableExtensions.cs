using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Infra
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> SafeConcat<T>(this IEnumerable<T> @this, IEnumerable<T> other)
        {
            return @this.Concat(other);
        }
    }
}
