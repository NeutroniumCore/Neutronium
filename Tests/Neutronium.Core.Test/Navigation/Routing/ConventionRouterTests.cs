using Neutronium.Core.Navigation;
using Neutronium.Core.Navigation.Routing;
using NSubstitute;
using Xunit;

namespace Neutronium.Core.Test.Navigation.Routing
{
    public class ConventionRouterTests
    {
        private readonly INavigationBuilder _NavigationBuilder;
        private readonly ConventionRouter _ConventionRouterWithId;
        private readonly ConventionRouter _ConventionRouterWithoutId;

        private class TestViewModel { };

        private class FooViewModel { };

        public ConventionRouterTests()
        {
            _NavigationBuilder = Substitute.For<INavigationBuilder>();
            _ConventionRouterWithId = new ConventionRouter(_NavigationBuilder, @"View\{vm}\{id}\dist\index.html");
            _ConventionRouterWithoutId = new ConventionRouter(_NavigationBuilder, @"View\{vm}\dist\index.html");
        }

        [Theory]
        [InlineData(null, @"View\Foo\dist\index.html")]
        [InlineData("edit", @"View\Foo\edit\dist\index.html")]
        [InlineData("delete", @"View\Foo\delete\dist\index.html")]
        public void Register_Call_NavigationBuilder_WithCorrectParameters_TemplateWithId(string id, string expectedPath)
        {
            RegisterAndCheck<FooViewModel>(_ConventionRouterWithId, id, expectedPath);
        }

        [Theory]
        [InlineData(null, @"View\Test\dist\index.html")]
        [InlineData("edit", @"View\Test\dist\index.html")]
        public void Register_Call_NavigationBuilder_WithCorrectParameters_TemplateWithoutId(string id, string expectedPath)
        {
            RegisterAndCheck<TestViewModel>(_ConventionRouterWithoutId, id, expectedPath);
        }

        private void RegisterAndCheck<T>(ConventionRouter router, string id, string expectedPath)
        {
            router.Register<T>(id);
            _NavigationBuilder.Received(1).Register<T>(expectedPath, id);
        }
    }
}
