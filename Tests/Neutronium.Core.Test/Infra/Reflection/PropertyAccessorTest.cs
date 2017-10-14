using System.Diagnostics;
using System.Reflection;
using FluentAssertions;
using Neutronium.Core.Infra.Reflection;
using Neutronium.Core.Test.Helper;
using Xunit;
using Xunit.Abstractions;

namespace Neutronium.Core.Test.Infra.Reflection
{
    public class PropertyAccessorTest
    {
        private readonly ITestOutputHelper _Output;

        public PropertyAccessorTest(ITestOutputHelper output)
        {
            _Output = output;
        }    

        private static PropertyAccessor GetPropertyAccessorFor(string propertyName)
        {
            var type = typeof(FakeClass);
            var propertyInfo = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            return new PropertyAccessor(typeof(FakeClass), new PropertyInfoDescription(propertyInfo), 0);
        }

        [Theory]
        [InlineData("Available2", true)]
        [InlineData("Available1", false)]
        [InlineData("Available3", false)]
        public void IsSettable_returns_correct_result(string property, bool isSettable)
        {
            var target = GetPropertyAccessorFor(property);
            target.IsSettable.Should().Be(isSettable);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(23)]
        [InlineData(42)]
        public void Get_returns_correct_result(int value)
        {
            var source = new FakeClass
            {
                Available2 = value
            };
            var target = GetPropertyAccessorFor("Available2");
            target.Get(source).Should().Be(value);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(23)]
        [InlineData(42)]
        public void Set_set_correct_value(int value)
        {
            var source = new FakeClass();
            var target = GetPropertyAccessorFor("Available2");
            target.Set(source,value);

            source.Available2.Should().Be(value);
        }

        [Fact]
        public void Get_Performance_Test_Stress()
        {
            var @object = new FakeClass();

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var operations = 100000000;
            var propertyInfo = typeof(FakeClass).GetProperty("Available2", BindingFlags.Public | BindingFlags.Instance);
            var description = new PropertyInfoDescription(propertyInfo);
            var myTypeInstrospector = new PropertyAccessor(typeof(FakeClass), description, 0);

            for (var i = 0; i < operations; i++)
            {         
                var res = myTypeInstrospector.Get(@object);
            }

            stopWatch.Stop();
            var ts = stopWatch.ElapsedMilliseconds;
            _Output.WriteLine($"Perf: {operations* 1000/ts} operations per sec");
        }
    }
}
