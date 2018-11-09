using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Infra
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> SafeConcat<T>(this IEnumerable<T> @this, IEnumerable<T> other)
        {
            var res = @this;
            if (res == null) {
                res = other;
            } else if (other != null) {
                res = @this.Concat(other);
            }
            return res ?? Enumerable.Empty<T>();
        }
    }
}
