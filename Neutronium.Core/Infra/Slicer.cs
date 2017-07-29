using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Infra
{
    public class Slicer<T>
    {
        private readonly IList<T> _Collection;
        private readonly int _MaxCount;

        public Slicer(IList<T> collection, int maxCount)
        {
            _Collection = collection;
            _MaxCount = maxCount;
        }

        public IEnumerable<T[]> Slice()
        {
            T[] res = null;
            var count = 0;
            do
            {
                res = _Collection.Skip(count).Take(_MaxCount).ToArray();
                if (res.Length != 0)
                    yield return res;
                count += _MaxCount;
            }
            while (res.Length == _MaxCount);
        }
    }
}
