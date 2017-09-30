using System.Collections.Generic;
using FluentAssertions;
using Neutronium.Core.Infra;
using Xunit;

namespace Neutronium.Core.Test.Infra
{
    public class ListExtensionsTest
    {
        [Theory]
        [InlineData(new[] { 1 }, 0, 2, new[] { 2 })]
        [InlineData(new[] { 1, 0, 3 }, 1, 2, new[] { 1, 2, 3 })]
        [InlineData(new[] { 1, 0 }, 1, 2, new[] { 1, 2 })]
        [InlineData(new[] { 1, 2, 3, 0 }, 3, 4, new[] { 1, 2, 3, 4 })]
        public void Apply_set_value_when_insert_is_false(IList<int> list, int index, int value, IList<int> expected)
        {
            ApplyAndTest(list, index, value, expected, false);
        }

        [Theory]
        [InlineData(new[] { 1 }, 0, 2, new[] { 2, 1 })]
        [InlineData(new[] { 1, 0, 3 }, 1, 2, new[] { 1, 2, 0, 3 })]
        [InlineData(new[] { 1, 0 }, 2, 2, new[] { 1, 0, 2 })]
        public void Apply_insert_value_when_insert_is_true(IList<int> array, int index, int value, IList<int> expected)
        {
            var list = new List<int>(array);
            ApplyAndTest(list, index, value, expected, true);
        }
      
        private void ApplyAndTest(IList<int> list, int index, int value, IList<int> expected, bool insert)
        {
            var indexDescriptor = new IndexDescriptor(index, insert);
            list.Apply(indexDescriptor, value);
            list.Should().Equal(expected);
        }
    }
}
