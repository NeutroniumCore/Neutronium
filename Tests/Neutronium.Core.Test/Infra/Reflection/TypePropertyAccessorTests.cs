using System;
using System.Linq;
using FluentAssertions;
using Neutronium.Core.Infra.Reflection;
using Xunit;

namespace Neutronium.Core.Test.Infra.Reflection
{
    public class TypePropertyAccessorTest
    {
        [Theory]
        [InlineData(typeof(FakeClass), new[] { "Available1", "Available2", "Available3" })]
        [InlineData(typeof(ReadOnlyClass2), new[] { "Property1", "Property2" })]
        public void AttributeNames_returns_correct_property(Type type, string[] properties)
        {
            var res = TypePropertyAccessor.FromType(type);
            res.AttributeNames.Should().BeEquivalentTo(properties);
        }

        [Theory]
        [InlineData(typeof(FakeClass), new[] { "Available1", "Available2", "Available3" })]
        [InlineData(typeof(ReadOnlyClass2), new[] { "Property1", "Property2" })]
        public void ReadProperties_returns_correct_property_name(Type type, string[] properties)
        {
            var res = TypePropertyAccessor.FromType(type);
            res.ReadProperties.Select(prop => prop.Name).Should().BeEquivalentTo(properties);
        }

        [Theory]
        [InlineData(typeof(FakeClass), new[] { 0, 1, 2 })]
        [InlineData(typeof(ReadOnlyClass2), new[] { 0, 1 })]
        public void ReadProperties_returns_correct_property_position(Type type, int[] positions)
        {
            var res = TypePropertyAccessor.FromType(type);
            res.ReadProperties.Select(prop => prop.Position).Should().BeEquivalentTo(positions);
        }

        [Theory]
        [InlineData(typeof(ReadOnlyClass), false)]
        [InlineData(typeof(FakeClass), true)]
        public void HasReadWriteProperties_returns_correct_property(Type type, bool expectedHasReadWriteProperties)
        {
            var res = TypePropertyAccessor.FromType(type);
            res.HasReadWriteProperties.Should().Be(expectedHasReadWriteProperties);
        }

        [Theory]
        [InlineData(typeof(FakeClass), "Available1")]
        [InlineData(typeof(FakeClass), "Available2")]
        [InlineData(typeof(FakeClass), "Available3")]
        public void GetAccessor_returns_correct_object(Type type, string propertyName)
        {
            var res = TypePropertyAccessor.FromType(type);
            var accesor = res.GetAccessor(propertyName);
            accesor.Name.Should().Be(propertyName);
        }

        [Theory]
        [InlineData(typeof(FakeClass), "Aailable1")]
        [InlineData(typeof(FakeClass), "NotFound")]
        [InlineData(typeof(FakeClass), "")]
        public void GetAccessor_returns_null_when_property_not_found(Type type, string propertyName)
        {
            var res = TypePropertyAccessor.FromType(type);
            var accesor = res.GetAccessor(propertyName);
            accesor.Should().BeNull();
        }
    }
}
