using System.Linq;
using FluentAssertions;
using Neutronium.Core.Infra;
using Xunit;

namespace Neutronium.Core.Test.Infra
{
    public class CollectionExtensionsTest
    {
        [Theory]
        [InlineData(new[] { 2, 1 })]
        [InlineData(new[] { 1, 2, 0, 3 })]
        [InlineData(new[] { 1, 0, 2 })]
        public void ToArray_transform_into_array(int[] array)
        {
            var result = array.ToArray(i => i.ToString());
            result.Should().Equal(array.Select(i => i.ToString()));
        }
    }
}
