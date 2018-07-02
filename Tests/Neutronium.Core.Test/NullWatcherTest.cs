using System;
using Xunit;
using Neutronium.Core.Log;

namespace Neutronium.Core.Test
{
    public class NullWatcherTest
    {
        private readonly IWebSessionLogger _NullLogger = new BasicLogger();
       
        [Fact]
        public void LogCriticalTest()
        {
            _NullLogger.Error(string.Empty);
        }

        [Fact]
        public void LogBrowserTest()
        {
            _NullLogger.LogBrowser(null, null);
        }

        [Fact]
        public void OnSessionErrorTest()
        {
            _NullLogger.WebBrowserError(new Exception(), () => { });
        }
    }
}
