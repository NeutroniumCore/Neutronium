using System;
using System.Linq;
using FluentAssertions;
using Neutronium.Core.Infra.Reflection;
using Neutronium.Core.Test.Helper;
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
            var res = new TypePropertyAccessor(type);
            res.AttributeNames.Should().BeEquivalentTo(properties);
        }

        [Theory]
        [InlineData(typeof(FakeClass), new[] { "Available1", "Available2", "Available3" })]
        [InlineData(typeof(ReadOnlyClass2), new[] { "Property1", "Property2" })]
        public void ReadProperties_returns_correct_property_name(Type type, string[] properties)
        {
            var res = new TypePropertyAccessor(type);
            res.ReadProperties.Select(prop => prop.Name).Should().BeEquivalentTo(properties);
        }

        [Theory]
        [InlineData(typeof(FakeClass), new[] { 0, 1, 2 })]
        [InlineData(typeof(ReadOnlyClass2), new[] { 0, 1 })]
        public void ReadProperties_returns_correct_property_position(Type type, int[] positions)
        {
            var res = new TypePropertyAccessor(type);
            res.ReadProperties.Select(prop => prop.Position).Should().BeEquivalentTo(positions);
        }

        [Theory]
        [InlineData(typeof(FakeClass), ObjectObservability.None)]
        [InlineData(typeof(ReadOnlyClass), ObjectObservability.ReadOnly)]       
        [InlineData(typeof(ReadWriteClassWithNotifyPropertyChanged), ObjectObservability.Observable)]
        [InlineData(typeof(ReadOnlyClassWithNotifyPropertyChanged), ObjectObservability.ReadOnlyObservable)]
        public void Observability_returns_correct_property(Type type, ObjectObservability expected)
        {
            var res = new TypePropertyAccessor(type);
            res.Observability.Should().Be(expected);
        }

        [Theory]
        [InlineData(typeof(FakeClass), "Available1")]
        [InlineData(typeof(FakeClass), "Available2")]
        [InlineData(typeof(FakeClass), "Available3")]
        public void GetAccessor_returns_correct_object(Type type, string propertyName)
        {
            var res = new TypePropertyAccessor(type);
            var accesor = res.GetAccessor(propertyName);
            accesor.Name.Should().Be(propertyName);
        }

        [Theory]
        [InlineData(typeof(FakeClass), "Aailable1")]
        [InlineData(typeof(FakeClass), "NotFound")]
        [InlineData(typeof(FakeClass), "")]
        public void GetAccessor_returns_null_when_property_not_found(Type type, string propertyName)
        {
            var res = new TypePropertyAccessor(type);
            var accesor = res.GetAccessor(propertyName);
            accesor.Should().BeNull();
        }

        [Fact]
        public void GetAccessor_returns_null_when_property_is_not_bindable()
        {
            var res = new TypePropertyAccessor(typeof(ClassWithAttributes));
            var accesor = res.GetAccessor(nameof(ClassWithAttributes.NotBindable));
            accesor.Should().BeNull();
        }

        [Theory]
        [InlineData(nameof(ClassWithAttributes.OneWay), false)]
        [InlineData(nameof(ClassWithAttributes.TwoWay), true)]
        public void GetAccessor_returns_null_use_bindable_direction_for_settable_value(string propertyName, bool settable)
        {
            var res = new TypePropertyAccessor(typeof(ClassWithAttributes));
            var accesor = res.GetAccessor(propertyName);
            accesor.IsSettable.Should().Be(settable);
        }
    }
}
