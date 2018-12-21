using FluentAssertions;
using Neutronium.Core;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.Log;
using NSubstitute;
using Xunit;

namespace Neutronium.WPF.Test
{
    public class HTMLEngineFactoryTest 
    {
        private readonly IWPFWebWindowFactory _WPFWebWindowFactory;
        private readonly IJavascriptFrameworkManager _JavascripEngine1;
        private readonly IJavascriptFrameworkManager _JavascripEngine2;
        private readonly IJavascriptFrameworkManager _JavascripEngine3;
        private readonly HTMLEngineFactory _HTMLEngineFactory = new HTMLEngineFactory();
        private readonly IWebSessionLogger _WebSessionLogger;

        public HTMLEngineFactoryTest() 
        {
            _WPFWebWindowFactory = CreateFactory("FakeWEngine");
            _JavascripEngine1 = CreateJavascriptFactory("One");
            _JavascripEngine2 = CreateJavascriptFactory("Two");
            _JavascripEngine3 = CreateJavascriptFactory("Three");
            _WebSessionLogger = Substitute.For<IWebSessionLogger>();
        }

        private IWPFWebWindowFactory CreateFactory(string name)
        {
            var factory = Substitute.For<IWPFWebWindowFactory>();
            factory.Name.Returns(name);
            return factory;
        }

        private IJavascriptFrameworkManager CreateJavascriptFactory(string name) 
        {
            var factory = Substitute.For<IJavascriptFrameworkManager>();
            factory.Name.Returns(name);
            return factory;
        }

        [Fact]
        public void WebSessionWatcher_IsInitializedWithNullWatcher() 
        {
            _HTMLEngineFactory.WebSessionLogger.Should().BeOfType<BasicLogger>();
        }

        [Fact]
        public void WebSessionWatcher_SetEngineWatcher_WhenWebSessionWatcher_IsCalledBeforeRegisterHTMLEngine() 
        {
            _HTMLEngineFactory.WebSessionLogger = _WebSessionLogger;
            _HTMLEngineFactory.RegisterHTMLEngine(_WPFWebWindowFactory);

            _WPFWebWindowFactory.WebSessionLogger.Should().Be(_WebSessionLogger);
        }

        [Fact]
        public void WebSessionWatcher_SetEngineWatcher_WhenWebSessionWatcher_IsCalledAfterRegisterHTMLEngine() 
        {
            _HTMLEngineFactory.RegisterHTMLEngine(_WPFWebWindowFactory);
            _HTMLEngineFactory.WebSessionLogger = _WebSessionLogger;
           
            _WPFWebWindowFactory.WebSessionLogger.Should().Be(_WebSessionLogger);
        }

        [Fact]
        public void ResolveJavaScriptFramework_WhenOneEngineIsRegistered_ReturnsThisElement() 
        {
             _HTMLEngineFactory.RegisterJavaScriptFramework(_JavascripEngine1);
            var res = _HTMLEngineFactory.ResolveJavaScriptFramework("");
            res.Should().Be(_JavascripEngine1);
        }

        [Fact]
        public void ResolveJavaScriptFramework_WhenVariopusEngineAreRegistered_ReturnsElementByName() 
        {
            _HTMLEngineFactory.RegisterJavaScriptFramework(_JavascripEngine1);
            _HTMLEngineFactory.RegisterJavaScriptFramework(_JavascripEngine2);

            var res = _HTMLEngineFactory.ResolveJavaScriptFramework(_JavascripEngine2.Name);
            res.Should().Be(_JavascripEngine2);
        }

        [Fact]
        public void ResolveJavaScriptFramework_WhenVariousEngineAreRegisteredAndNameNotFound_ReturnsNull() 
        {
            _HTMLEngineFactory.RegisterJavaScriptFramework(_JavascripEngine1);
            _HTMLEngineFactory.RegisterJavaScriptFramework(_JavascripEngine2);

            var res = _HTMLEngineFactory.ResolveJavaScriptFramework("NotFound");
            res.Should().BeNull();
        }

        [Fact]
        public void ResolveJavaScriptFramework_WhenDefaultIsSet_ReturnsDefaultIfElementNotFound() 
        {
            _HTMLEngineFactory.RegisterJavaScriptFramework(_JavascripEngine1);
            _HTMLEngineFactory.RegisterJavaScriptFramework(_JavascripEngine2);
            _HTMLEngineFactory.RegisterJavaScriptFrameworkAsDefault(_JavascripEngine3);

            var res = _HTMLEngineFactory.ResolveJavaScriptFramework("NotFound");
            res.Should().Be(_JavascripEngine3);
        }
    }
}
