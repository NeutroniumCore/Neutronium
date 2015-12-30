using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;
using NSubstitute;
using Xunit;
using MVVM.HTML.Core;
using MVVM.HTML.Core.Navigation;

namespace MVVM.HTML.Core.Test
{
    public class NullWatcherTest
    {
        private IWebSessionWatcher _NullWatcher = new NullWatcher();
       
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
