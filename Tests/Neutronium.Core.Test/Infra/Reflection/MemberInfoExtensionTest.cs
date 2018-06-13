using System.ComponentModel;
using FluentAssertions;
using Neutronium.Core.Infra.Reflection;
using Neutronium.Core.Test.Helper;
using Xunit;

namespace Neutronium.Core.Test.Infra.Reflection
{
    public class MemberInfoExtensionTest
    {
        [Theory]
        [InlineData(nameof(ClassWithAttributes.OneWay), true, BindingDirection.OneWay)]
        [InlineData(nameof(ClassWithAttributes.TwoWay), true, BindingDirection.TwoWay)]
        [InlineData(nameof(ClassWithAttributes.NotBindable), false, BindingDirection.OneWay)]
        public void GetAttribute_returns_attribute(string name, bool bindable, BindingDirection direction)
        {
            var expected = new BindableAttribute(bindable, direction);
            var propertyInfo = typeof(ClassWithAttributes).GetProperty(name);
            var attribute = propertyInfo.GetAttribute<BindableAttribute>();

            attribute.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetAttribute_returns_null_if_not_such_attribute()
        {
            var propertyInfo = typeof(ClassWithAttributes).GetProperty(nameof(ClassWithAttributes.NoAttribute));
            var attribute = propertyInfo.GetAttribute<BindableAttribute>();

            attribute.Should().BeNull();
        }
    }
}
