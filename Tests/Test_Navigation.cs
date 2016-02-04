using System;
using Xunit;
using FluentAssertions;
using MVVM.HTML.Core;

namespace MVVM.Cef.Glue.Test
{
    public class Test_Navigation
    {
        private NavigationBuilder _INavigationBuilder;

        public Test_Navigation()
        {
            _INavigationBuilder = new NavigationBuilder();
        }

        public class A1
        {
        }

        [Fact]
        public void Test_WPFBrowserNavigator_Register_ShouldNotAceptBadPath()
        {
            _INavigationBuilder.Should().NotBeNull();
                    Action wf = () =>
                        {
                            _INavigationBuilder.Register<A1>("javascript\\navigationk_1.html");
                        };
                    wf.ShouldThrow<Exception>();

                    wf = () =>
                    {
                        _INavigationBuilder.RegisterAbsolute<A1>("C:\\javascript\\navigationk_1.html");
                    };
                    wf.ShouldThrow<Exception>();

                    wf = () =>
                    {
                        _INavigationBuilder.Register<A1>(new Uri("C:\\navigationk_1.html"));
                    };
                    wf.ShouldThrow<Exception>();
        }
    }
}
