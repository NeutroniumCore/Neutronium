using System;
using System.Threading;
using FluentAssertions;
using Tests.Infra.HTMLEngineTesterHelper.Window;
using Xunit;

namespace Tests.Infra.Tests
{
    public class WpfThreadTest : IDisposable
    {
        private readonly WpfThread _wpfThread;
        public WpfThreadTest()
        {
            _wpfThread = new WpfThread();
        }

        [Fact]
        public void Constructor_Populate_Dispatcher()
        {
            _wpfThread.Dispatcher.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_Populate_UIThread()
        {
            _wpfThread.UIThread.Should().NotBeNull();
        }

        [Fact]
        public void Dispatcher_Should_BeFunctional()
        {
            Thread thread = null;
            Action action = () => thread = Thread.CurrentThread;
            _wpfThread.Dispatcher.Invoke(action);
            thread.Should().Be(_wpfThread.UIThread);
        }

        public void Dispose()
        {
            _wpfThread.Dispose();
        }
    }
}
