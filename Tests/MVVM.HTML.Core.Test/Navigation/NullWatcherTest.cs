using System;
using MVVM.HTML.Core.Infra;
using Xunit;
using MVVM.HTML.Core.Navigation;

namespace MVVM.HTML.Core.Test
{
    public class NullWatcherTest
    {
        private readonly IWebSessionLogger _nullLogger = new BasicLogger();
       
        [Fact]
        public void Test_LogCritical()
        {
            _nullLogger.Error(string.Empty);
        }

        [Fact]
        public void Test_LogBrowser()
        {
            _nullLogger.LogBrowser(null, null);
        }


        [Fact]
        public void Test_OnSessionError()
        {
            _nullLogger.WebBrowserError(new Exception(), () => { });
        }
    }
}
