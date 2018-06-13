using System;
using FluentAssertions;
using Neutronium.Core.Navigation;
using Xunit;

namespace Neutronium.Core.Test 
{
    public class NavigationBuilderTests
    {
        private NavigationBuilder _NavigationBuilder;
        public NavigationBuilderTests()
        {
            _NavigationBuilder = new NavigationBuilder();
        }

        [Fact]
        public void Register_Twice_Should_Generate_Error()
        {
            _NavigationBuilder.Register<object>("javascript\\index.html");

            Action Failed = () => _NavigationBuilder.Register<object>("javascript\\index.html");

            Failed.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Solve_Should_Use_Default_Register_WhenId_NotFound()
        {
            _NavigationBuilder.Register<object>("javascript\\index.html");

            var Uri = _NavigationBuilder.Solve( new object(),"pathnotused");

            Uri.LocalPath.Should().EndWith("javascript\\index.html");
        }

        [Fact]
        public void Solve_ShoulWork_WithDefaultId()
        {
            _NavigationBuilder.Register<object>("javascript\\index.html");

            var Uri = _NavigationBuilder.Solve(new object());

            Uri.LocalPath.Should().EndWith("javascript\\index.html");
        }

        [Fact]
        public void Register_ShouldNotAceptBadPath()
        {

            _NavigationBuilder.Should().NotBeNull();
            Action wf = () =>
            {
                _NavigationBuilder.Register<object>("javascript\\navigationk_1.html");
            };
            wf.Should().Throw<Exception>();

            wf = () =>
            {
                _NavigationBuilder.RegisterAbsolute<object>("C:\\javascript\\navigationk_1.html");
            };
            wf.Should().Throw<Exception>();

            wf = () =>
            {
                _NavigationBuilder.Register<object>(new Uri("C:\\navigationk_1.html"));
            };
            wf.Should().Throw<Exception>();
        }
    }
}
