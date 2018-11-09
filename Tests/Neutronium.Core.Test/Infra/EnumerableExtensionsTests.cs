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
        public void SafeConcat_concatenate_collection(List<int> first, List<int> second)
        {
            var res = first.SafeConcat(second);
            res.Should().BeEquivalentTo(first.Concat(second));
        }
    }
}
