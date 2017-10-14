using System.ComponentModel;
using FluentAssertions;
using Neutronium.Core.Infra.Reflection;
using Neutronium.Core.Test.Helper;
using Xunit;

namespace Neutronium.Core.Test.Infra.Reflection
{
    public class PropertyInfoDescriptionTests
    {
        [Theory]
        [InlineData(nameof(ClassWithAttributes.OneWay), true, BindingDirection.OneWay)]
        [InlineData(nameof(ClassWithAttributes.TwoWay), true, BindingDirection.TwoWay)]
        [InlineData(nameof(ClassWithAttributes.NotBindable), false, BindingDirection.OneWay)]
        public void Constructor_builds_correct_object(string name, bool bindable, BindingDirection direction)
        {
            var expected = new BindableAttribute(bindable, direction);
            var propertyInfo = typeof(ClassWithAttributes).GetProperty(name);
            var propertyInfoDescription = new PropertyInfoDescription(propertyInfo);

            propertyInfoDescription.Attribute.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public void Constructor_builds_correct_object()
        {
            var propertyInfo = typeof(ClassWithAttributes).GetProperty(nameof(ClassWithAttributes.NoAttribute));
            var propertyInfoDescription = new PropertyInfoDescription(propertyInfo);

            propertyInfoDescription.Attribute.Should().BeNull();
        }
    }
}
