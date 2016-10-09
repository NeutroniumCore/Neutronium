using System;
using System.Collections.Generic;
using FluentAssertions;
using Neutronium.Core.Infra;
using Xunit;

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

        [Fact]
        public void GetUnderlyingNullableType_null()
        {
            Type n = null;
            n.GetUnderlyingNullableType().Should().BeNull();
        }

        [Fact]
        public void GetUnderlyingiliststring_null()
        {
            Type n = typeof(IList<string>);
            n.GetUnderlyingNullableType().Should().BeNull();
        }

        [Fact]
        public void GetUnderlyingNullableType_int()
        {
            typeof(Nullable<int>).GetUnderlyingNullableType().Should().Be(typeof(int));
        }

        [Fact]
        public void GetUnderlyingNullableType_string()
        {
            typeof(string).GetUnderlyingNullableType().Should().BeNull();
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
    }
}
