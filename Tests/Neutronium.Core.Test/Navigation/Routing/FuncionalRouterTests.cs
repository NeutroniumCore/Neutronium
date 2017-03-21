using Neutronium.Core.Navigation;
using Neutronium.Core.Navigation.Routing;
using NSubstitute;
using System;
using Xunit;

namespace Neutronium.Core.Test.Navigation.Routing
{
    public class FuncionalRouterTests
    {
        private readonly INavigationBuilder _NavigationBuilder;
        private readonly Func<Type, string, string> _PathBuilder;
        private readonly FuncionalRouter _FuncionalRouter;

        private class TestViewModel { };

        public FuncionalRouterTests()
        {
            _NavigationBuilder = Substitute.For<INavigationBuilder>();
            _PathBuilder = Substitute.For<Func<Type, string, string>>();
            _FuncionalRouter = new FuncionalRouter(_NavigationBuilder, _PathBuilder);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("edit")]
        [InlineData("delete")]
        public void Register_Call_PathBuilder_WithCorrectArguments(string id)
        {
            _FuncionalRouter.Register<TestViewModel>(id);
            _PathBuilder.Received(1).Invoke(typeof(TestViewModel), id);
        }


        [Theory]
        [InlineData(null, "path1")]
        [InlineData("edit", "path2")]
        [InlineData("delete", "path3")]
        public void Register_Call_NavigationBuilder_WithPathBuilderResult(string id, string result)
        {
            _PathBuilder.Invoke(typeof(TestViewModel), id).Returns(result);
            _FuncionalRouter.Register<TestViewModel>(id);
            _NavigationBuilder.Received(1).Register<TestViewModel>(result, id);
        }
    }
}
