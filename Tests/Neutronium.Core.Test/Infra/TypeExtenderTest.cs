using System;
using System.Collections.Generic;
using FluentAssertions;
using Neutronium.Core.Infra;
using Xunit;
using System.Linq;

namespace Neutronium.Core.Test.Infra
{
    public class TypeExtenderTest
    {
        [Fact]
        public void GetEnumerableBase_int_isNotEnumerable()
        {
            typeof(int).GetEnumerableBase().Should().BeNull();
        }

        [Fact]
        public void GetEnumerableBase_IEnumerable_int_isNotEnumerable()
        {
            typeof(IEnumerable<int>).GetEnumerableBase().Should().Be(typeof(int));
        }

        [Fact]
        public void GetEnumerableBase_IList_int_isNotEnumerable()
        {
            typeof(IList<int>).GetEnumerableBase().Should().Be(typeof(int));
        }

        [Fact]
        public void GetEnumerableBase_null_int_isNotEnumerable()
        {
            Type n = null;
            n.GetEnumerableBase().Should().BeNull();
        }

        [Theory]
        [InlineData(typeof(Nullable<int>), typeof(int))]
        [InlineData(typeof(Nullable<double>), typeof(double))]
        public void GetUnderlyingNullableType_Returns_Expected_Value(Type type, Type expected)
        {
            type.GetUnderlyingNullableType().Should().Be(expected);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(typeof(IList<string>))]
        [InlineData(typeof(string))]
        public void GetUnderlyingNullableType_Returns_Expected_Null_Value(Type type)
        {
            type.GetUnderlyingNullableType().Should().BeNull();
        }

        [Theory]
        [InlineData(typeof(Nullable<int>), typeof(int))]
        [InlineData(typeof(Nullable<double>), typeof(double))]
        [InlineData(typeof(IList<string>), typeof(IList<string>))]
        [InlineData(typeof(string), typeof(string))]
        public void GetUnderlyingType_Returns_Expected_Null_Value(Type type, Type expected)
        {
            type.GetUnderlyingType().Should().Be(expected);
        }

        [Theory]
        [InlineData(typeof(UInt16), true)]
        [InlineData(typeof(UInt32), true)]
        [InlineData(typeof(UInt64), true)]
        [InlineData(typeof(Int16), false)]
        [InlineData(typeof(Int32), false)]
        [InlineData(typeof(Int64), false)]
        [InlineData(null, false)]
        public void IsUnsigned_returns_expected_value(Type type, bool result)
        {
            var isUnsigned = type.IsUnsigned();
            isUnsigned.Should().Be(result);
        }

        [Theory]
        [InlineData(typeof(FakeClass), new[] { "Available1", "Available2", "Available3" })]
        [InlineData(typeof(ReadOnlyClass2), new[] { "Property1", "Property2" })]    
        public void GetReadProperties_returns_correct_property(Type type, string[] properties)
        {
            var res = type.GetReadProperties();
            res.Select(prop => prop.Name).Should().BeEquivalentTo(properties);
        }

        [Fact]
        public void GetReadProperties_returns_null_when_called_with_null()
        {
            var res = TypeExtender.GetReadProperties(null);
            res.Should().BeNull();
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(typeof(ReadOnlyClass), false)]
        [InlineData(typeof(FakeClass), true)]
        public void HasReadWriteProperties_returns_correct_property(Type type, bool expectedHasReadWriteProperties)
        {
            var res = type.HasReadWriteProperties();
            res.Should().Be(expectedHasReadWriteProperties);
        }
    }
}
