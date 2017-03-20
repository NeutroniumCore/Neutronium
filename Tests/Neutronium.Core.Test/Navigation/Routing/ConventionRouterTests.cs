using Neutronium.Core.Navigation;
using Neutronium.Core.Navigation.Routing;
using NSubstitute;
using Xunit;

namespace Neutronium.Core.Test.Navigation.Routing {
    public class ConventionRouterTests 
    { 
        private readonly INavigationBuilder _NavigationBuilder;
        private readonly ConventionRouter _ConventionRouter;

        private class TestViewModel {};

        public ConventionRouterTests() 
        {
            _NavigationBuilder = Substitute.For<INavigationBuilder>();
            _ConventionRouter = new ConventionRouter(_NavigationBuilder, @"View\{vm}\{id}\dist\index.html");
        }

        [Theory]
        [InlineData(null, @"View\Test\dist\index.html")]
        [InlineData("edit", @"View\Test\edit\dist\index.html")]
        [InlineData("delete", @"View\Test\delete\dist\index.html")]
        public void Register_Call_NavigationBuilder_WithCorrectParameters(string id, string path) 
        {
            _ConventionRouter.Register<TestViewModel>(id);
            _NavigationBuilder.Received(1).Register<TestViewModel>(Arg.Is<string>(p => p== path), id);
        }
    }
}
