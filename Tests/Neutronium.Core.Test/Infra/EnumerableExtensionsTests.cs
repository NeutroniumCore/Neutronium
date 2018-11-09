using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using FluentAssertions;
using Neutronium.Core.Infra;
using Xunit;

namespace Neutronium.Core.Test.Infra
{
    public class EnumerableExtensionsTests
    {
        [Theory, AutoData]
        public void SafeConcat_concatenates_collection(List<int> first, List<int> second)
        {
            var res = first.SafeConcat(second);
            res.Should().BeEquivalentTo(first.Concat(second));
        }

        [Theory]
        [InlineData(null, null, new int[0] { })]
        [InlineData(null, new[] { 1, 2}, new [] { 1, 2 })]
        [InlineData(new[] { 3, 1, 2 }, null, new int[] { 3, 1, 2 })]
        public void SafeConcat_can_be_used_with_null(IList<int> first, IList<int> second, IList<int> expected) {
            var res = first.SafeConcat(second);
            res.Should().BeEquivalentTo(expected);
        }
    }
}
