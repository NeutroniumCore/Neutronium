using System.Collections.Generic;
using FluentAssertions;
using Neutronium.Core.Infra;
using Xunit;

namespace Neutronium.Core.Test.Infra
{
    public class DictionaryExtensionsTest
    {
        private readonly Dictionary<string, int> _Dictionary = new Dictionary<string, int>
        {
            ["One"] = 1,
            ["Two"] = 2
        };

        [Theory]
        [InlineData("One", 1)]
        [InlineData("Two", 2)]
        public void GetOrNull_returns_dictionary_value(string key, object expectedResult)
        {
            var result = _Dictionary.GetOrNull(key);
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("Three")]
        [InlineData("Four")]
        public void GetOrNull_returns_null_if_value_not_found(string key)
        {
            var result = _Dictionary.GetOrNull(key);
            result.Should().BeNull();
        }
    }
}
