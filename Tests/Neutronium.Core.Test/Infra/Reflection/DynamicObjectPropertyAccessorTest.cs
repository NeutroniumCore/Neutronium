using System.Dynamic;
using System.Linq;
using FluentAssertions;
using Neutronium.Core.Infra;
using Neutronium.Core.Infra.Reflection;
using Neutronium.Core.Test.Helper;
using Xunit;

namespace Neutronium.Core.Test.Infra.Reflection
{
    public class DynamicObjectPropertyAccessorTest
    {
        private readonly DynamicObject _DynamicObject;
        private readonly DynamicObjectPropertyAccessor _DynamicObjectPropertyAccessor;

        public DynamicObjectPropertyAccessorTest()
        {
            _DynamicObject = new DynamicObjectTest();
            _DynamicObjectPropertyAccessor = new DynamicObjectPropertyAccessor(_DynamicObject);
        }

        [Fact]
        public void AttributeNames_has_correct_value()
        {
            _DynamicObjectPropertyAccessor.AttributeNames.Should().Equal("classic", "int", "ReadOnlyByAttribute", "ReadOnlyByNature", "size", "string");
        }

        [Fact]
        public void Observability_is_none()
        {
            _DynamicObjectPropertyAccessor.Observability.Should().Be(ObjectObservability.None);
        }

        [Fact]
        public void ReadProperties_are_ordered_by_position()
        {
            _DynamicObjectPropertyAccessor.ReadProperties.Select(p => p.Position).Should().Equal(0, 1, 2, 3, 4, 5);
        }

        [Fact]
        public void ReadProperties_are_ordered_by_name()
        {
            _DynamicObjectPropertyAccessor.ReadProperties.Select(p => p.Name).Should().Equal("classic", "int", "ReadOnlyByAttribute", "ReadOnlyByNature", "size", "string");
        }

        [Fact]
        public void ReadProperties_have_functional_getters()
        {
            _DynamicObjectPropertyAccessor.ReadProperties.Select(p => p.Get(_DynamicObject)).Should().Equal(23, 3, null, null, 4, 6);
        }

        [Fact]
        public void ReadProperties_have_functional_setters()
        {
            _DynamicObjectPropertyAccessor.GetAccessor("size").Set(_DynamicObject,25);
            dynamic dynamycObject = _DynamicObject;
            int value = dynamycObject.size;

            value.Should().Be(25);
        }

        [Theory]
        [InlineData("size", 4)]
        [InlineData("string", 5)]
        [InlineData("int", 1)]
        [InlineData("classic", 0)]
        public void GetAccessor_returns_properties_with_correct_position(string propertyName, int index)
        {
            var result = _DynamicObjectPropertyAccessor.GetAccessor(propertyName);
            var expected = _DynamicObjectPropertyAccessor.ReadProperties[index];
            result.Should().BeSameAs(expected);
            result.Position.Should().Be(index);
        }

        [Theory]
        [InlineData("size", 4)]
        [InlineData("string", 5)]
        [InlineData("int", 1)]
        [InlineData("classic", 0)]      
        public void GetAccessor_returns_cached_accessor_when_any(string propertyName, int index)
        {
            var result = _DynamicObjectPropertyAccessor.GetAccessor(propertyName);
            var expected = _DynamicObjectPropertyAccessor.ReadProperties[index];
            result.Should().BeSameAs(expected);
            result.Position.Should().Be(index);
        }

        [Theory]
        [InlineData("Five")]
        [InlineData("Six")]
        public void GetAccessor_returns_null_when_not_found(string propertyName)
        {
            var result = _DynamicObjectPropertyAccessor.GetAccessor(propertyName);
            result.Should().BeNull();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void GetIndex_return_original_index_on_old_element(int index)
        {
            var propertyAcessor = _DynamicObjectPropertyAccessor.ReadProperties[index];

            var indexValue = _DynamicObjectPropertyAccessor.GetIndex(propertyAcessor);
            indexValue.Should().Be(new IndexDescriptor(index));
        }

        [Fact]
        public void DynamicObjectPropertyAccessor_constructor_prioritize_static_over_dynamicProperties()
        {
            var ambigeousDynamicObject = new AmbigeousDynamicObject();
            var dynamicObjectPropertyAccessor = new DynamicObjectPropertyAccessor(ambigeousDynamicObject);

            var acessor =  dynamicObjectPropertyAccessor.GetAccessor("Ambigeous");
            var result = acessor.Get(ambigeousDynamicObject);

            result.Should().Be("static");
        }

        [Fact]
        public void Clr_dynamicObject_prioritize_static_over_dynamicProperties()
        {
            dynamic ambigeousDynamicObject = new AmbigeousDynamicObject();
            string result = ambigeousDynamicObject.Ambigeous;
            result.Should().Be("static");
        }
    }
}
