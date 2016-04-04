using FluentAssertions;
using MVVM.HTML.Core;
using MVVM.HTML.Core.Navigation;
using NSubstitute;
using Xunit;

namespace HTML_WPF.Component.Test 
{
    public class HTMLEngineFactoryTest 
    {
        private readonly IWPFWebWindowFactory _WPFWebWindowFactory;
        private HTMLEngineFactory _HTMLEngineFactory = new HTMLEngineFactory();
        private readonly IWebSessionWatcher _IWebSessionWatcher;

        public HTMLEngineFactoryTest() 
        {
            _WPFWebWindowFactory = Substitute.For<IWPFWebWindowFactory>();
            _WPFWebWindowFactory.Name.Returns("FakeWEngine");
            _IWebSessionWatcher = Substitute.For<IWebSessionWatcher>();
        }

        [Fact]
        public void WebSessionWatcher_IsInitializedWithNullWatcher() 
        {
            _HTMLEngineFactory.WebSessionWatcher.Should().BeOfType<NullWatcher>();
        }

        [Fact]
        public void WebSessionWatcher_SetEngineWatcher_WhenWebSessionWatcher_IsCalledBeforeRegisterHTMLEngine() 
        {
            _HTMLEngineFactory.WebSessionWatcher = _IWebSessionWatcher;
            _HTMLEngineFactory.RegisterHTMLEngine(_WPFWebWindowFactory);

            _WPFWebWindowFactory.WebSessionWatcher.Should().Be(_IWebSessionWatcher);
        }

        [Fact]
        public void WebSessionWatcher_SetEngineWatcher_WhenWebSessionWatcher_IsCalledAfterRegisterHTMLEngine() 
        {
            _HTMLEngineFactory.RegisterHTMLEngine(_WPFWebWindowFactory);
            _HTMLEngineFactory.WebSessionWatcher = _IWebSessionWatcher;
           
            _WPFWebWindowFactory.WebSessionWatcher.Should().Be(_IWebSessionWatcher);
        }
    }
}
