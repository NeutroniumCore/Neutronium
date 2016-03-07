using System;
using System.Threading;
using FluentAssertions;
using IntegratedTest.WPF.Infra;
using Xunit;

namespace IntegratedTests.Tests
{
    public class WPFThreadHelperTest : IDisposable
    {
        private readonly WPFThreadHelper _WPFThreadHelper;
        public WPFThreadHelperTest()
        {
            _WPFThreadHelper = new WPFThreadHelper();
        }

        [Fact]
        public void Constructor_Populate_Dispatcher()
        {
            _WPFThreadHelper.Dispatcher.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_Populate_UIThread()
        {
            _WPFThreadHelper.UIThread.Should().NotBeNull();
        }

        [Fact]
        public void Dispatcher_Should_BeFunctional()
        {
            Thread thread = null;
            Action action = () => thread = Thread.CurrentThread;
            _WPFThreadHelper.Dispatcher.Invoke(action);
            thread.Should().Be(_WPFThreadHelper.UIThread);
        }

        public void Dispose()
        {
            _WPFThreadHelper.Dispose();
        }
    }
}
