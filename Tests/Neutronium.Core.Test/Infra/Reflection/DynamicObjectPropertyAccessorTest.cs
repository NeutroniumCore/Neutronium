using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using FluentAssertions;
using Neutronium.Core.Infra;
using Neutronium.Core.Infra.Reflection;
using Xunit;

namespace Neutronium.Core.Test.Infra.Reflection
{
    public class DynamicObjectPropertyAccessorTest
    {
        private readonly DynamicObject _DynamicObject;
        private readonly DynamicObjectPropertyAccessor _DynamicObjectPropertyAccessor;

        private class MyDynamicObject : DynamicObject
        {
            private readonly Dictionary<string, int> _Dictionary = new Dictionary<string, int>
            {
                ["string"] = 6,
                ["int"] = 3,
                ["size"] = 4
            };

            public override IEnumerable<string> GetDynamicMemberNames() => _Dictionary.Keys;

            public int classic => 23;
      
            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                int res;
                if (_Dictionary.TryGetValue(binder.Name, out res))
                {
                    result = res;
                    return true;
                }
                result = null;
                return false;
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                _Dictionary[binder.Name] = (int)value;
                return true;
            }
        }

        public DynamicObjectPropertyAccessorTest()
        {
            _DynamicObject = new MyDynamicObject();
            _DynamicObjectPropertyAccessor = new DynamicObjectPropertyAccessor(_DynamicObject);
        }

        [Fact]
        public void AttributeNames_has_correct_value()
        {
            _DynamicObjectPropertyAccessor.AttributeNames.Should().Equal("classic", "int", "size", "string");
        }

        [Fact]
        public void HasReadWriteProperties_Is_True()
        {
            _DynamicObjectPropertyAccessor.HasReadWriteProperties.Should().BeTrue();
        }

        [Fact]
        public void ReadProperties_are_ordered_by_position()
        {
            _DynamicObjectPropertyAccessor.ReadProperties.Select(p => p.Position).Should().Equal(0, 1, 2, 3);
        }

        [Fact]
        public void ReadProperties_are_ordered_by_name()
        {
            _DynamicObjectPropertyAccessor.ReadProperties.Select(p => p.Name).Should().Equal("classic", "int", "size", "string");
        }

        [Fact]
        public void ReadProperties_have_functional_getters()
        {
            _DynamicObjectPropertyAccessor.ReadProperties.Select(p => (int)p.Get(_DynamicObject)).Should().Equal(23, 3, 4, 6);
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
        [InlineData("size", 2)]
        [InlineData("string", 3)]
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


        private class AmbigeousDynamicObject : DynamicObject
        {
            public override IEnumerable<string> GetDynamicMemberNames() => new [] { "Ambigeous" };

            public string Ambigeous => "static";

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = "dynamic";
                return true;
            }
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
