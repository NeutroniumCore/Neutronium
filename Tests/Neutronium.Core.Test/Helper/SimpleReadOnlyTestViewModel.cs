using System;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Test.Helper
{
    public class SimpleReadOnlyTestViewModel
    {
        private static readonly Random _Random = new Random();

        public SimpleReadOnlyTestViewModel[] Children { get; }

        public int One => 1;
        public int Two => 2;

        public int RandomNumber { get; }

        public SimpleReadOnlyTestViewModel(IEnumerable<SimpleReadOnlyTestViewModel> children)
        {
            RandomNumber = _Random.Next();
            Children = children?.ToArray();
        }

        public static SimpleReadOnlyTestViewModel BuildBigVm(int leaves = 50, int position = 3)
        {
            var children = position == 0 ? null : Enumerable.Range(0, leaves).Select(i => BuildBigVm(leaves, position - 1));
            return new SimpleReadOnlyTestViewModel(children);
        }
    }
}
