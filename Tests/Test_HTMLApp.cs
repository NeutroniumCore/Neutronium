using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NSubstitute;
using Xunit;
using FluentAssertions;

using MVVM.CEFGlue.CefSession;
using System.Windows;
using System.Threading;
using Xilium.CefGlue.WPF;
using CefGlue.Window;

namespace MVVM.CEFGlue.Test
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

        [Fact]
        public async Task Test_HTMLApp_Start_Should_not_Override_Session()
        {
            using (var ctx = new EnvironmentBuilder())
            {
                HTMLApp target = null;
                IDispatcher disp = await Task.Run(() =>
                {
                    target = new HTMLApp();
                    return new WPFUIDispatcher(target.Dispatcher);

                });

                Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    CefCoreSessionSingleton.Session.Should().Be(ctx.Session);
                    disp.Run(() => target.Shutdown(0));
                });

                disp.Run(() => target.Run());
            }
        }


        [Fact]
        public async Task Test_HTMLApp_Start_Should_Create_Session()
        {
            HTMLApp target = null;
            IDispatcher disp = await Task.Run(() =>
            {
                target = new HTMLApp();
                return new WPFUIDispatcher(target.Dispatcher);
            });

            Task.Run(() =>
            {
                Thread.Sleep(1000);
                CefCoreSessionSingleton.Session.Should().NotBeNull();
                disp.Run(() => target.Shutdown(0));
            });

            disp.Run(() => target.Run());

        }

    }
}
