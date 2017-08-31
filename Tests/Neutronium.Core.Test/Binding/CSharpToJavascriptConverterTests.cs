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

        public CSharpToJavascriptConverterTests()
        {
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
        public void CreateJSGlueObject_WithCorrectToString_ListSimple()
        {
            var testObject = new TestClass();
            testObject.Children.Add(testObject);
            var res = _CSharpToJavascriptConverter.Map(testObject);

            res.ToString().Should().Be("{\"Children\":[\"~\"],\"Property1\":null,\"Property2\":null,\"Property3\":null}");
        }

        [Fact]
        public void CreateJSGlueObject_WithCorrectToString_TransformingQuote()
        {
            var testObject = new StringClass
            {
                Value = @"a""quote"""
            };
            var res = _CSharpToJavascriptConverter.Map(testObject);

            res.ToString().Should().Be(@"{""Value"":""a\""quote\""""}");
        }

        [Fact]
        public void CreateJSGlueObject_WithCorrectToString_TransformingSlash()
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

        private class TestClass
        {
            public List<TestClass> Children { get; } = new List<TestClass>();
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
