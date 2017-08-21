using System;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Example.ViewModel
{
    public class FakeViewModel
    {
        private static readonly Random Random = new Random();

        public FakeViewModel[] Children { get; }

        public int One => 1;
        public int Two => 2;
        public int RandomNumber { get; }

        public FakeViewModel(IEnumerable<FakeViewModel> children = null )
        {
            RandomNumber = Random.Next();
            Children = children?.ToArray();
        }
    }
}
