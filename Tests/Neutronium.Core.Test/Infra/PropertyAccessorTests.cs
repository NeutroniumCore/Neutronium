using FluentAssertions;
using Neutronium.Core.Infra;
using NSubstitute;
using System;
using Xunit;

namespace Neutronium.Core.Test.Infra
{
    public class PropertyAccessorTests
    {
        private readonly IWebSessionLogger _Logger;
        public PropertyAccessorTests()
        {
            _Logger = Substitute.For<IWebSessionLogger>();
        }

        [Theory]
        [InlineData(typeof(FakeClass), "Available1", true)]
        [InlineData(typeof(FakeClass), "Available2", true)]
        [InlineData(typeof(FakeClass), "Available3", true)]
        [InlineData(typeof(FakeClass), "", false)]
        [InlineData(typeof(FakeClass), "invalid", false)]
        public void IsValid_returns_correct_false(Type type, string property, bool valid)
        {
            var @object = Activator.CreateInstance(type);
            var target = new PropertyAccessor(@object, property, _Logger);
            target.IsValid.Should().Be(valid);
        }

        [Theory]
        [InlineData(typeof(FakeClass), "Available1", true)]
        [InlineData(typeof(FakeClass), "Available2", true)]
        [InlineData(typeof(FakeClass), "Available3", true)]
        [InlineData(typeof(FakeClass), "Private1", false)]
        [InlineData(typeof(FakeClass), "Private2", false)]
        [InlineData(typeof(FakeClass), "Protected1", false)]
        [InlineData(typeof(FakeClass), "Protected2", false)]
        [InlineData(typeof(FakeClass), "OnlySet", false)]
        [InlineData(typeof(FakeClass), "", false)]
        [InlineData(typeof(FakeClass), "invalid", false)]
        public void IsGettable_returns_correct_false(Type type, string property, bool isGettable)
        {
            var @object = Activator.CreateInstance(type);
            var target = new PropertyAccessor(@object, property, _Logger);
            target.IsGettable.Should().Be(isGettable);
        }

        [Theory]
        [InlineData(typeof(FakeClass), "Protected2", true)]
        [InlineData(typeof(FakeClass), "OnlySet", true)]
        [InlineData(typeof(FakeClass), "Available2", true)]
        [InlineData(typeof(FakeClass), "Private2", true)]
        [InlineData(typeof(FakeClass), "Available1", false)]
        [InlineData(typeof(FakeClass), "Available3", false)]
        [InlineData(typeof(FakeClass), "Private1", false)]    
        [InlineData(typeof(FakeClass), "Protected1", false)]
        [InlineData(typeof(FakeClass), "", false)]
        [InlineData(typeof(FakeClass), "invalid", false)]
        public void IsSettable_returns_correct_false(Type type, string property, bool isSettable)
        {
            var @object = Activator.CreateInstance(type);
            var target = new PropertyAccessor(@object, property, _Logger);
            target.IsSettable.Should().Be(isSettable);
        }
    }
}
