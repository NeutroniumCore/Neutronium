using Neutronium.Core.Binding;
using Neutronium.Core.Binding.GlueObject;
using NSubstitute;
using Xunit;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.Test.TestHelper;
using System.Threading.Tasks;
using System.Collections.Generic;
using FluentAssertions;

namespace Neutronium.Core.Test.Binding
{
    public class CSharpToJavascriptConverterTests
    {
        private CSharpToJavascriptConverter _CSharpToJavascriptConverter;
        private IJavascriptSessionCache _Cacher;
        private IJSCommandFactory _CommandFactory;
        private IWebSessionLogger _Logger;
        private IWebView _WebView;
        private IDispatcher _UiDispatcher;
        private IJavascriptFrameworkManager _JavascriptFrameworkManager;
        private IJavascriptChangesObserver _JavascriptChangesObserver;

        private HTMLViewContext _HTMLViewContext;

        public CSharpToJavascriptConverterTests()
        {
            _Cacher = Substitute.For<IJavascriptSessionCache>();
            IJSCSGlue res = null;
            _Cacher.GetCached(Arg.Any<object>()).Returns(res);
            _CommandFactory = Substitute.For<IJSCommandFactory>();
            _Logger = Substitute.For<IWebSessionLogger>();
            _HTMLViewContext = GetContext();
            _CSharpToJavascriptConverter = new CSharpToJavascriptConverter(_HTMLViewContext, _Cacher, _CommandFactory, _Logger);
        }

        private HTMLViewContext GetContext()
        {
            _WebView = Substitute.For<IWebView>();
            _JavascriptFrameworkManager = Substitute.For<IJavascriptFrameworkManager>();
            _JavascriptChangesObserver = Substitute.For<IJavascriptChangesObserver>();
            _UiDispatcher = new TestUIDispatcher();
            var res = new HTMLViewContext(_WebView, _UiDispatcher, _JavascriptFrameworkManager, _JavascriptChangesObserver, _Logger);
            return res;
        }

        [Fact]
        public async Task Map_CreateJSGlueObject_WithCorrectToString()
        {
            var testObject = new TestClass();
            var res = await _CSharpToJavascriptConverter.Map(testObject);

            res.ToString().Should().Be("{\"Children\":[],\"Property1\":null,\"Property2\":null,\"Property3\":null}");
        }


        private class TestClass
        {
            public List<TestClass> Children { get; } = new List<TestClass>();
            public TestClass Property1 { get; set; }
            public TestClass Property2 { get; set; }
            public TestClass Property3 { get; set; }           
        }
    }
}
