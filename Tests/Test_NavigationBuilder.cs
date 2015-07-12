using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;
using FluentAssertions;

namespace MVVM.CEFGlue.Test
{
    public class Test_NavigationBuilder
    {
        private NavigationBuilder _NavigationBuilder;
        public Test_NavigationBuilder()
        {
            _NavigationBuilder = new NavigationBuilder();
        }

        [Fact]
        public void Test_DoubleRegister_Shoul_Generate_Error()
        {
            _NavigationBuilder.Register<object>("javascript\\index.html");

            Action Failed = () => _NavigationBuilder.Register<object>("javascript\\index.html");

            Failed.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Test_WPFBrowserNavigator_Register_Should_Use_Default_Register_WhenId_NotFound()
        {
            _NavigationBuilder.Register<object>("javascript\\index.html");

            var Uri = _NavigationBuilder.Solve( new object(),"pathnotused");

            Uri.LocalPath.Should().EndWith("javascript\\index.html");
        }

        [Fact]
        public void Test_WPFBrowserNavigator_Register_ShouldNotAceptBadPath()
        {

            _NavigationBuilder.Should().NotBeNull();
            Action wf = () =>
            {
                _NavigationBuilder.Register<object>("javascript\\navigationk_1.html");
            };
            wf.ShouldThrow<Exception>();

            wf = () =>
            {
                _NavigationBuilder.RegisterAbsolute<object>("C:\\javascript\\navigationk_1.html");
            };
            wf.ShouldThrow<Exception>();

            wf = () =>
            {
                _NavigationBuilder.Register<object>(new Uri("C:\\navigationk_1.html"));
            };
            wf.ShouldThrow<Exception>();
        }
    }
}
