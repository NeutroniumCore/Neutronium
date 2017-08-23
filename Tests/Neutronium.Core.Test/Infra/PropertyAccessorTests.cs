using FluentAssertions;
using Neutronium.Core.Infra;
using Neutronium.Core.Infra.Reflection;
using System.Diagnostics;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Neutronium.Core.Test.Infra
{
    public class PropertyAccessorTests
    {
        private readonly ITestOutputHelper _Output;

        public PropertyAccessorTests(ITestOutputHelper output)
        {
            _Output = output;
        }    

        private static PropertyAccessor GetPropertyAccessorFor(string propertyName)
        {
            var type = typeof(FakeClass);
            var propertyInfo = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            return new PropertyAccessor(typeof(FakeClass), propertyInfo);
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

        [Fact]
        public void Get_Performance_Test_Stress()
        {
            var @object = new FakeClass();

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            for (var i = 0; i < 10000000; i++)
            {
                var myTypeInstrospector = @object.GetType().GetReadProperty("Available2");
                var res = myTypeInstrospector.Get(@object);
            }

            stopWatch.Stop();
            var ts = stopWatch.ElapsedMilliseconds;
            _Output.WriteLine($"Perf: {((double)(ts)) / 1000} sec");
        }
    }
}
