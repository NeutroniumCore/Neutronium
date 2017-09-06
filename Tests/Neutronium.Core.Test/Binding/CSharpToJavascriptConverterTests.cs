using System;
using Neutronium.Core.Binding;
using Neutronium.Core.Binding.GlueObject;
using NSubstitute;
using Xunit;
using Neutronium.Core.WebBrowserEngine.Window;
using System.Collections.Generic;
using FluentAssertions;
using MoreCollection.Extensions;
using Neutronium.Core.Binding.GlueObject.Factory;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.Test.Helper;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Xunit.Abstractions;

namespace Neutronium.Core.Test.Binding
{
    public class CSharpToJavascriptConverterTests
    {
        private readonly CSharpToJavascriptConverter _CSharpToJavascriptConverter;
        private readonly IJavascriptSessionCache _Cacher;
        private readonly IGlueFactory _GlueFactory;
        private readonly IWebSessionLogger _Logger;
        private readonly Dictionary<object, IJsCsGlue> _Cache = new Dictionary<object, IJsCsGlue>();
        private readonly IWebBrowserWindow _WebBrowserWindow;
        private readonly ObjectChangesListener _ObjectChangesListener;
        private readonly ITestOutputHelper _TestOutputHelper;

        public CSharpToJavascriptConverterTests(ITestOutputHelper testOutputHelper)
        {
            _TestOutputHelper = testOutputHelper;
            _Cacher = Substitute.For<IJavascriptSessionCache>();
            _Cacher.When(c => c.CacheFromCSharpValue(Arg.Any<object>(), Arg.Any<IJsCsGlue>()))
                   .Do(callInfo => _Cache.Add(callInfo[0], (IJsCsGlue)callInfo[1]));
            _Cacher.GetCached(Arg.Any<object>()).Returns(callInfo => _Cache.GetOrDefault(callInfo[0]));
            _ObjectChangesListener = new ObjectChangesListener(_ => { }, _ => {}, _ => { });
            _GlueFactory = new GlueFactory(null, _Cacher, null, _ObjectChangesListener);
            _Logger = Substitute.For<IWebSessionLogger>();
            _WebBrowserWindow = Substitute.For<IWebBrowserWindow>();
            _WebBrowserWindow.IsTypeBasic(typeof(string)).Returns(true);
            _CSharpToJavascriptConverter = new CSharpToJavascriptConverter(_WebBrowserWindow, _Cacher, _GlueFactory, _Logger);
        }

        [Fact]
        public void Map_CreateJSGlueObject_WithCorrectToString_NoneCircular()
        {
            var testObject = new TestClass();
            var res = _CSharpToJavascriptConverter.Map(testObject);

            res.ToString().Should().Be("{\"Children\":[],\"Property1\":null,\"Property2\":null,\"Property3\":null}");
        }

        [Fact]
        public void Map_CreateJSGlueObject_WithCorrectToString_Nested()
        {
            var testObject = new TestClass
            {
                Property1 = new TestClass()
            };
            var res = _CSharpToJavascriptConverter.Map(testObject);

            res.ToString().Should().Be("{\"Children\":[],\"Property1\":{\"Children\":[],\"Property1\":null,\"Property2\":null,\"Property3\":null},\"Property2\":null,\"Property3\":null}");
        }

        [Fact]
        public void Map_CreateJSGlueObject_WithCorrectToString_CircularRoot()
        {
            var testObject = new TestClass();
            testObject.Property1 = testObject;
            var res = _CSharpToJavascriptConverter.Map(testObject);

            res.ToString().Should().Be("{\"Children\":[],\"Property1\":\"~\",\"Property2\":null,\"Property3\":null}");
        }

        [Fact]
        public void Map_CreateJSGlueObject_WithCorrectToString_CircularProperty()
        {
            var testObject = new TestClass();
            var testObject2 = new TestClass();
            testObject.Property1 = testObject2;
            testObject.Property2 = testObject2;
            var res = _CSharpToJavascriptConverter.Map(testObject);

            res.ToString().Should().Be("{\"Children\":[],\"Property1\":{\"Children\":[],\"Property1\":null,\"Property2\":null,\"Property3\":null},\"Property2\":\"~Property1\",\"Property3\":null}");
        }

        [Fact]
        public void Map_CreateJSGlueObject_WithCorrectToString_CircularNestedProperty()
        {
            var testObject = new TestClass();
            var testObject2 = new TestClass();
            testObject.Property1 = new TestClass
            {
                Property2 = testObject2
            };
            testObject.Property3 = testObject2;
            var res = _CSharpToJavascriptConverter.Map(testObject);

            res.ToString().Should().Be("{\"Children\":[],\"Property1\":{\"Children\":[],\"Property1\":null,\"Property2\":{\"Children\":[],\"Property1\":null,\"Property2\":null,\"Property3\":null},\"Property3\":null},\"Property2\":null,\"Property3\":\"~Property1~Property2\"}");
        }

        [Fact]
        public void Map_CreateJSGlueObject_WithCorrectToString_CircularListProperty()
        {
            var testObject = new TestClass();
            var tesObject2 = new TestClass();
            var children = new List<TestClass> { tesObject2 };
            testObject.Children = children;
            tesObject2.Children = children;
            var res = _CSharpToJavascriptConverter.Map(testObject);

            res.ToString().Should().Be("{\"Children\":[{\"Children\":\"~Children\",\"Property1\":null,\"Property2\":null,\"Property3\":null}],\"Property1\":null,\"Property2\":null,\"Property3\":null}");
        }

        [Fact]
        public void Map_CreateJSGlueObject_WithCorrectToString_ListSimple()
        {
            var testObject = new TestClass();
            testObject.Children.Add(testObject);
            var res = _CSharpToJavascriptConverter.Map(testObject);

            res.ToString().Should().Be("{\"Children\":[\"~\"],\"Property1\":null,\"Property2\":null,\"Property3\":null}");
        }

        [Fact]
        public void Map_CreateJSGlueObject_WithCorrectToString_TransformingQuote()
        {
            var testObject = new StringClass
            {
                Value = @"a""quote"""
            };
            var res = _CSharpToJavascriptConverter.Map(testObject);

            res.ToString().Should().Be(@"{""Value"":""a\""quote\""""}");
        }

        [Fact]
        public void Map_CreateJSGlueObject_WithCorrectToString_TransformingSlash()
        {
            var testObject = new StringClass
            {
                Value = @"C:\Users\David\Documents\Source\DiscogsClient\DiscogsClient\bin\Debug\DiscogsClient.dll"
            };
            var res = _CSharpToJavascriptConverter.Map(testObject);

            res.ToString().Should().Be(@"{""Value"":""C:\\Users\\David\\Documents\\Source\\DiscogsClient\\DiscogsClient\\bin\\Debug\\DiscogsClient.dll""}");
        }

        [Fact]
        public void Map_CreateJSGlueObject_WithCorrectToString_ListProperty()
        {
            var testObject = new TestClass();
            var testObject2 = new TestClass();
            testObject.Property3 = testObject2;
            testObject.Children.Add(testObject2);
            var res = _CSharpToJavascriptConverter.Map(testObject);

            res.ToString().Should().Be("{\"Children\":[{\"Children\":[],\"Property1\":null,\"Property2\":null,\"Property3\":null}],\"Property1\":null,\"Property2\":null,\"Property3\":\"~Children~0\"}");
        }


        [Fact]
        public void Map_performance_test()
        {
            var converter = GetCSharpToJavascriptConverterForPerformance();
            var vm = SimpleReadOnlyTestViewModel.BuildBigVm();

            using (GetPerformanceCounter("Map large Vm"))
            {
                var res = converter.Map(vm);
            }
        }

        protected PerformanceHelper GetPerformanceCounter(string description) => new PerformanceHelper(_TestOutputHelper, description);


        private CSharpToJavascriptConverter GetCSharpToJavascriptConverterForPerformance()
        {
            var cacher = new SessionCacher();
            var factory = new GlueFactory(null, cacher, null, _ObjectChangesListener);
            return new CSharpToJavascriptConverter(new WebBrowserWindowFakde(), cacher, factory, _Logger);
        }

        private class WebBrowserWindowFakde : IWebBrowserWindow
        {
            private readonly HashSet<Type> _BasicTypes = new HashSet<Type>
            {
                typeof(string),
                typeof(int),
                typeof(char),
                typeof(double),
                typeof(DateTime)
            };
            public IWebView MainFrame { get; }
            public bool IsTypeBasic(Type type) => _BasicTypes.Contains(type);
            public void NavigateTo(Uri path)
            {
            }

            public Uri Url { get; }
            public bool IsLoaded { get; }
            public event EventHandler<LoadEndEventArgs> LoadEnd;
            public event EventHandler<ConsoleMessageArgs> ConsoleMessage;
            public event EventHandler<BrowserCrashedArgs> Crashed;
        }


        private class TestClass
        {
            public List<TestClass> Children { get; set; } = new List<TestClass>();
            public TestClass Property1 { get; set; }
            public TestClass Property2 { get; set; }
            public TestClass Property3 { get; set; }           
        }

        private class StringClass
        {
            public string Value { get; set; }
        }
    }
}
