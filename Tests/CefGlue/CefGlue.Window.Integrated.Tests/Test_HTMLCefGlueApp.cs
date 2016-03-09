using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HTML_WPF.Component;
using KnockoutUIFramework;
using MVVM.Cef.Glue;
using MVVM.Cef.Glue.CefSession;
using NSubstitute;
using Xunit;

namespace CefGlue.Window.Integrated.Tests 
{
    public class Test_HTMLCefGlueApp
    {
        private class EnvironmentBuilder : IDisposable
        {
            public EnvironmentBuilder()
            {
                Session = Substitute.For<ICefCoreSession>();
                CefCoreSessionSingleton.Session = Session;
            }

            public ICefCoreSession Session { get; private set; }

            public void Dispose()
            {
                CefCoreSessionSingleton.Session = null;
            }
        }

        private static HTMLCefGlueApp GetApplication()
        {
            return new HTMLCefGlueApp() { JavascriptUiFrameworkManager = new KnockoutUiFrameworkManager() };
        }

        [Fact(Skip = "One at a time")]
        public void Test_HTMLApp_Start_Should_not_Override_Session()
        {
            using (var ctx = new EnvironmentBuilder())
            {
                var target = GetApplication();
                var disp = new WPFUIDispatcher(target.Dispatcher);

                Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    CefCoreSessionSingleton.Session.Should().Be(ctx.Session);
                    disp.Run(() => target.Shutdown(0));
                });

                target.Run();
            }
        }


        [Fact(Skip = "One at a time")]
        public void Test_HTMLApp_Start_Should_Create_Session()
        {
            var target = GetApplication();
            var disp = new WPFUIDispatcher(target.Dispatcher);

            Task.Run(() =>
            {
                Thread.Sleep(500);
                CefCoreSessionSingleton.Session.Should().NotBeNull();
                disp.Run(() => target.Shutdown(0));
            });

            target.Run();
        }
    }
}
