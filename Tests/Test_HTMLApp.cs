using System;
using System.Threading.Tasks;

using NSubstitute;
using Xunit;
using FluentAssertions;

using MVVM.Cef.Glue.CefSession;
using System.Threading;
using MVVM.Cef.Glue.WPF;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptEngine.Window;
using KnockoutUIFramework;

namespace MVVM.Cef.Glue.Test
{
    public class Test_HTMLApp
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

        [Fact]
        public async Task Test_HTMLApp_Start_Should_not_Override_Session()
        {
            using (var ctx = new EnvironmentBuilder())
            {
                HTMLCefGlueApp target = null;
                IDispatcher disp = await Task.Run(() =>
                {
                    target = GetApplication();
                    return new WPFUIDispatcher(target.Dispatcher);

                });

                Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    CefCoreSessionSingleton.Session.Should().Be(ctx.Session);
                    disp.Run(() => target.Shutdown(0));
                }).DoNotWait();

                disp.Run(() => target.Run());
            }
        }


        [Fact]
        public async Task Test_HTMLApp_Start_Should_Create_Session()
        {
            HTMLCefGlueApp target = null;
            IDispatcher disp = await Task.Run(() =>
            {
                target = GetApplication();
                return new WPFUIDispatcher(target.Dispatcher);
            });

            Task.Run(() =>
            {
                Thread.Sleep(1000);
                CefCoreSessionSingleton.Session.Should().NotBeNull();
                disp.Run(() => target.Shutdown(0));
            }).DoNotWait();

            disp.Run(() => target.Run());
        }
    }
}
