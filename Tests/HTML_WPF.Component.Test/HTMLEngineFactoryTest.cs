using FluentAssertions;
using MVVM.HTML.Core;
using MVVM.HTML.Core.Infra;
using NSubstitute;
using Xunit;

namespace HTML_WPF.Component.Test 
{
    public class HTMLEngineFactoryTest 
    {
        private readonly IWPFWebWindowFactory _WPFWebWindowFactory;
        private readonly HTMLEngineFactory _HTMLEngineFactory = new HTMLEngineFactory();
        private readonly IWebSessionLogger _iWebSessionLogger;

        public HTMLEngineFactoryTest() 
        {
            _WPFWebWindowFactory = Substitute.For<IWPFWebWindowFactory>();
            _WPFWebWindowFactory.Name.Returns("FakeWEngine");
            _iWebSessionLogger = Substitute.For<IWebSessionLogger>();
        }

        [Fact]
        public void WebSessionWatcher_IsInitializedWithNullWatcher() 
        {
            _HTMLEngineFactory.WebSessionLogger.Should().BeOfType<BasicLogger>();
        }

        [Fact]
        public void WebSessionWatcher_SetEngineWatcher_WhenWebSessionWatcher_IsCalledBeforeRegisterHTMLEngine() 
        {
            _HTMLEngineFactory.WebSessionLogger = _iWebSessionLogger;
            _HTMLEngineFactory.RegisterHTMLEngine(_WPFWebWindowFactory);

            _WPFWebWindowFactory.WebSessionLogger.Should().Be(_iWebSessionLogger);
        }

        [Fact]
        public void WebSessionWatcher_SetEngineWatcher_WhenWebSessionWatcher_IsCalledAfterRegisterHTMLEngine() 
        {
            _HTMLEngineFactory.RegisterHTMLEngine(_WPFWebWindowFactory);
            _HTMLEngineFactory.WebSessionLogger = _iWebSessionLogger;
           
            _WPFWebWindowFactory.WebSessionLogger.Should().Be(_iWebSessionLogger);
        }
    }
}
