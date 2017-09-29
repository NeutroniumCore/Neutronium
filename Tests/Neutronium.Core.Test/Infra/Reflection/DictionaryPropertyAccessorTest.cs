using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MoreCollection.Extensions;
using Neutronium.Core.Infra;
using Neutronium.Core.Infra.Reflection;
using Xunit;

namespace Neutronium.Core.Test.Infra.Reflection
{
    public class DictionaryPropertyAccessorTest
    {
        private readonly Dictionary<string, int> _Dictionary = new Dictionary<string, int>
        {
            ["One"] = 1,
            ["Two"] = 2,
            ["Three"] = 3,
            ["Four"] = 4
        };

        private readonly DictionaryPropertyAccessor<int> _DictionaryPropertyAccessor;

        public DictionaryPropertyAccessorTest()
        {
            _DictionaryPropertyAccessor = new DictionaryPropertyAccessor<int>(_Dictionary);
        }

        [Fact]
        public void AttributeNames_has_correct_value()
        {
            _DictionaryPropertyAccessor.AttributeNames.Should().Equal("Four", "One", "Three", "Two");
        }

        [Fact]
        public void HasReadWriteProperties_Is_True()
        {
            _DictionaryPropertyAccessor.HasReadWriteProperties.Should().BeTrue();
        }

        [Fact]
        public void ReadProperties_are_ordered_by_position()
        {
            _DictionaryPropertyAccessor.ReadProperties.Select(p => p.Position).Should().Equal(0, 1, 2, 3);
        }

        [Fact]
        public void ReadProperties_are_ordered_by_name()
        {
            _DictionaryPropertyAccessor.ReadProperties.Select(p => p.Name).Should().Equal("Four", "One", "Three", "Two");
        }

        [Fact]
        public void ReadProperties_have_functional_getters()
        {
            _DictionaryPropertyAccessor.ReadProperties.Select(p => (int)p.Get(_Dictionary)).Should().Equal(4, 1, 3, 2);
        }

        [Fact]
        public void ReadProperties_have_functional_setters()
        {
            var init = 20;
            _DictionaryPropertyAccessor.ReadProperties.ForEach(p => p.Set(_Dictionary, init++));

            var expected = new Dictionary<string, int>
            {
                ["One"] = 21,
                ["Two"] = 23,
                ["Three"] = 22,
                ["Four"] = 20
            };

            _Dictionary.Should().Equal(expected);
        }

        [Theory]
        [InlineData("One", 1)]
        [InlineData("Two", 3)]
        [InlineData("Three", 2)]
        [InlineData("Four", 0)]
        public void GetAccessor_returns_cached_accessor_when_any(string propertyName, int index)
        {
            var result = _DictionaryPropertyAccessor.GetAccessor(propertyName);
            var expected = _DictionaryPropertyAccessor.ReadProperties[index];
            result.Should().BeSameAs(expected);
            result.Position.Should().Be(index);
        }

        [Theory]
        [InlineData("Five")]
        [InlineData("Six")]
        public void GetAccessor_returns_new_accessor_when_needed(string propertyName)
        {
            var result = _DictionaryPropertyAccessor.GetAccessor(propertyName);

            _DictionaryPropertyAccessor.ReadProperties.Should().HaveCount(4);
            _DictionaryPropertyAccessor.ReadProperties.Should().NotContain(result);
            result.Position.Should().Be(-1);
            result.Name.Should().Be(propertyName);
        }

        [Fact]
        public void GetAccessor_returns_new_functional_accessor_when_needed()
        {
            var result = _DictionaryPropertyAccessor.GetAccessor("Five");
            result.Set(_Dictionary, 5);

            var expected = new Dictionary<string, int>
            {
                ["One"] = 1,
                ["Two"] = 2,
                ["Three"] = 3,
                ["Four"] = 4,
                ["Five"] = 5
            };

            _Dictionary.Should().Equal(expected);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void GetIndex_return_original_index_on_old_element(int index)
        {
            var propertyAcessor = _DictionaryPropertyAccessor.ReadProperties[index];

            var indexValue = _DictionaryPropertyAccessor.GetIndex(propertyAcessor);
            indexValue.Should().Be(new IndexDescriptor(index));
        }

        [Theory]
        [InlineData("Five", 0)]
        [InlineData("Six", 2)]
        public void GetIndex_creates_index_when_needed(string propertyName, int expectedIndex)
        {
            var propertyAcessor = _DictionaryPropertyAccessor.GetAccessor(propertyName);

            var indexValue = _DictionaryPropertyAccessor.GetIndex(propertyAcessor);
            indexValue.Should().Be(new IndexDescriptor(expectedIndex, true));
        }

        [Theory]
        [InlineData("Five", new[] { "Five", "Four", "One", "Three", "Two"})]
        [InlineData("Six", new[] { "Four", "One", "Six",  "Three", "Two" })]
        [InlineData("Zen", new[] { "Four", "One", "Three", "Two", "Zen" })]
        public void GetIndex_alters_attributeNames_when_needed(string propertyName, string[] properties)
        {
            var propertyAcessor = _DictionaryPropertyAccessor.GetAccessor(propertyName);
            _DictionaryPropertyAccessor.GetIndex(propertyAcessor);

            _DictionaryPropertyAccessor.AttributeNames.Should().Equal(properties);
        }

        [Theory]
        [InlineData("Five", new[] { "Five", "Four", "One", "Three", "Two" })]
        [InlineData("Six", new[] { "Four", "One", "Six", "Three", "Two" })]
        [InlineData("Zen", new[] { "Four", "One", "Three", "Two", "Zen" })]
        public void GetIndex_alters_readProperties_when_needed(string propertyName, string[] properties)
        {
            var propertyAcessor = _DictionaryPropertyAccessor.GetAccessor(propertyName);
            _DictionaryPropertyAccessor.GetIndex(propertyAcessor);

            _DictionaryPropertyAccessor.ReadProperties.Select(p => p.Name).Should().Equal(properties);
        }

        [Theory]
        [InlineData("Five")]
        [InlineData("Six")]
        [InlineData("Zen")]
        public void GetIndex_maintains_position_consistency(string propertyName)
        {
            var propertyAcessor = _DictionaryPropertyAccessor.GetAccessor(propertyName);
            _DictionaryPropertyAccessor.GetIndex(propertyAcessor);

            _DictionaryPropertyAccessor.ReadProperties.Select(p => p.Position).Should().Equal(0, 1, 2, 3, 4);
        }

        [Fact]
        public void FromStringDictionary_returns_DictionaryPropertyAccessor()
        {
            var result = DictionaryPropertyAccessor.FromStringDictionary(_Dictionary, typeof(int));
            result.Should().BeAssignableTo<DictionaryPropertyAccessor<int>>();
        }
    }
}
