using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using FluentAssertions;
using Neutronium.Core.Infra;
using Neutronium.Core.Infra.Reflection;
using Neutronium.Core.Test.Helper;
using Xunit;

namespace Neutronium.Core.Test.Infra
{
    public class TypeExtensionsTest
    {   
        private class ListDecimal: List<decimal> { }
       
        private interface IFakeInterface<T1, T2, T3> {}
        private class FakeClass<T1, T2, T3> : IFakeInterface<T1, T2, T3> { }

        private interface IFakeInterface<T1, T2> { }
        private class FakeClass<T1, T2> : IFakeInterface<T1, T2> { }

        [Theory]
        [InlineData(typeof(IEnumerable<int>), typeof(int))]
        [InlineData(typeof(List<object>), typeof(object))]
        [InlineData(typeof(IList<string>), typeof(string))]
        [InlineData(typeof(int[]), typeof(int))]
        [InlineData(typeof(ListDecimal), typeof(decimal))]
        public void GetEnumerableBase_returns_collection_item_type(Type source, Type expected)
        {
            source.GetEnumerableBase().Should().Be(expected);
        }

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        [InlineData(typeof(IList))]
        [InlineData(null)]
        public void GetEnumerableBase_returns_null_when_no_match(Type source)
        {
            source.GetEnumerableBase().Should().BeNull();
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
        [InlineData(typeof(Dictionary<string, object>), typeof(object))]
        [InlineData(typeof(Dictionary<string, string>), typeof(string))]
        [InlineData(typeof(Dictionary<string, int>), typeof(int))]
        public void GetDictionaryStringValueType_Returns_Expected_Null_Value(Type type, Type expected)
        {
            type.GetDictionaryStringValueType().Should().Be(expected);
        }


        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        [InlineData(typeof(IList))]
        [InlineData(typeof(Dictionary<object,object>))]
        [InlineData(typeof(Dictionary<int, string>))]
        public void GetDictionaryStringValueType_returns_null_when_no_match(Type source)
        {
            source.GetDictionaryStringValueType().Should().BeNull();
        }

        [Theory]
        [InlineData(typeof(Dictionary<string, object>), typeof(IDictionary<,>), typeof(string))]
        [InlineData(typeof(List<int>), typeof(IList<>), typeof(int))]
        [InlineData(typeof(List<decimal>), typeof(IEnumerable<>), typeof(decimal))]
        public void GetInterfaceGenericType_Returns_Expected_Null_Value(Type type, Type genericInterfaceType, Type expected) 
        {
            type.GetInterfaceGenericType(genericInterfaceType).Should().Be(expected);
        }


        [Theory]
        [InlineData(typeof(Array), typeof(IList<>))]
        [InlineData(typeof(Dictionary<int, string>), typeof(IList<>))]
        [InlineData(typeof(List<int>), typeof(IDictionary<,>))]
        public void GetInterfaceGenericType_returns_null_when_no_match(Type source, Type genericInterfaceType) 
        {
            source.GetInterfaceGenericType(genericInterfaceType).Should().BeNull();
        }

        [Theory]
        [InlineData(typeof(Dictionary<string, object>), typeof(IDictionary<,>), typeof(string), typeof(object))]
        [InlineData(typeof(Dictionary<object, int>), typeof(IDictionary<,>), typeof(object), typeof(int))]
        [InlineData(typeof(FakeClass<double, char>), typeof(IFakeInterface<,>), typeof(double), typeof(char))]
        public void GetInterfaceGenericTypes_Returns_Expected_Null_Value(Type type, Type genericInterfaceType, Type firstExpected, Type secondExpected)
        {
            var expected = Tuple.Create(firstExpected, secondExpected);
            type.GetInterfaceGenericTypes(genericInterfaceType).Should().Be(expected);
        }

        [Theory]
        [InlineData(typeof(Array), typeof(IList<>))]
        [InlineData(typeof(Dictionary<int, string>), typeof(IList<>))]
        [InlineData(typeof(List<int>), typeof(IDictionary<,>))]
        [InlineData(typeof(List<int>), typeof(IList<>))]
        [InlineData(typeof(FakeClass<double, char, int>), typeof(IFakeInterface<,,>))]
        [InlineData(null, typeof(IDictionary<,>))]
        public void GetInterfaceGenericTypes_returns_null_when_no_match(Type source, Type genericInterfaceType)
        {
            source.GetInterfaceGenericTypes(genericInterfaceType).Should().BeNull();
        }

        [Fact]
        public void GetAttribute_returns_correct_value()
        {
            var expected = new BindableAttribute(true, BindingDirection.OneWay);
            var res = typeof(ClassWithAttributesAndDefaultAttribute).GetAttribute<BindableAttribute>();
            res.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetAttribute_returns_null_if_attribute_not_found()
        {
            var res = typeof(ClassWithAttributes).GetAttribute<BindableAttribute>();
            res.Should().BeNull();
        }

        [Fact]
        public void GetPropertyInfoDescriptions_returns_expected_value()
        {
            var type = typeof(ClassWithAttributes);
            var expected = new[]
            {
                new PropertyInfoDescription(type.GetProperty(nameof(ClassWithAttributes.NoAttribute))),
                new PropertyInfoDescription(type.GetProperty(nameof(ClassWithAttributes.OneWay))),
                new PropertyInfoDescription(type.GetProperty(nameof(ClassWithAttributes.TwoWay))), 
            };

            type.GetPropertyInfoDescriptions().Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetPropertyInfoDescriptions_returns_expected_value_using_default()
        {
            var type = typeof(ClassWithAttributesAndDefaultAttribute);
            var expected = new[]
            {
                new PropertyInfoDescription(type.GetProperty(nameof(ClassWithAttributes.NoAttribute)), new BindableAttribute(true, BindingDirection.OneWay)),
                new PropertyInfoDescription(type.GetProperty(nameof(ClassWithAttributes.OneWay))),
                new PropertyInfoDescription(type.GetProperty(nameof(ClassWithAttributes.TwoWay))),
            };

            type.GetPropertyInfoDescriptions().Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(typeof(Array), false)]
        [InlineData(typeof(Dictionary<int, string>), false)]
        [InlineData(typeof(ExpandoObject), true)]
        [InlineData(typeof(INotifyPropertyChanged), true)]
        [InlineData(typeof(ViewModelTestBase), true)]
        public void ImplementsNotifyPropertyChanged_return_true_when_implementes_INotifyPropertyChanged(Type source, bool expected)
        {
            source.ImplementsNotifyPropertyChanged().Should().Be(expected);
        }
    }
}
