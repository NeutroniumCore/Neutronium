using System;
using Xunit;
using MVVM.HTML.Core.Navigation;

namespace MVVM.HTML.Core.Test
{
    public class NullWatcherTest
    {
        private readonly IWebSessionWatcher _NullWatcher = new NullWatcher();
       
        [Fact]
        public void Test_LogCritical()
        {
            _NullWatcher.LogCritical(string.Empty);
        }

        [Fact]
        public void Test_LogBrowser()
        {
            _NullWatcher.LogBrowser(string.Empty);
        }


        [Fact]
        public void Test_OnSessionError()
        {
            _NullWatcher.OnSessionError(new Exception(), () => { });
        }
    }
}
