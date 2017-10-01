using Neutronium.Core.Binding;
using Neutronium.Core.Binding.GlueObject;
using NSubstitute;
using Xunit;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using FluentAssertions;
using MoreCollection.Extensions;
using Neutronium.Core.Binding.GlueBuilder;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.Test.Helper;
using Xunit.Abstractions;
using Newtonsoft.Json;

namespace Neutronium.Core.Test.Binding
{
    public class CSharpToJavascriptConverterTests
    {
        private readonly CSharpToJavascriptConverter _CSharpToJavascriptConverter;
        private readonly IJavascriptSessionCache _Cacher;
        private readonly IGlueFactory _GlueFactory;
        private readonly IWebSessionLogger _Logger;
        private readonly Dictionary<object, IJsCsGlue> _Cache = new Dictionary<object, IJsCsGlue>();
        private readonly ObjectChangesListener _ObjectChangesListener;
        private readonly ITestOutputHelper _TestOutputHelper;

        public CSharpToJavascriptConverterTests(ITestOutputHelper testOutputHelper)
        {
            _TestOutputHelper = testOutputHelper;
            _Cacher = Substitute.For<IJavascriptSessionCache>();
            _Cacher.When(c => c.CacheFromCSharpValue(Arg.Any<object>(), Arg.Any<IJsCsGlue>()))
                   .Do(callInfo => _Cache.Add(callInfo[0], (IJsCsGlue)callInfo[1]));
            _Cacher.GetCached(Arg.Any<object>()).Returns(callInfo => _Cache.GetOrDefault(callInfo[0]));
            _ObjectChangesListener = new ObjectChangesListener(_ => { }, _ => { }, _ => { });
            _GlueFactory = new GlueFactory(null, _Cacher, null, _ObjectChangesListener);
            _Logger = Substitute.For<IWebSessionLogger>();
            _CSharpToJavascriptConverter = new CSharpToJavascriptConverter(_Cacher, _GlueFactory, _Logger);
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
        public void Map_maps_nullable_value()
        {
            var vm = new NullableTestViewModel
            {
                Bool = true,
                Decimal = 0.01m,
                Double = 0.9221,
                Int32 = 1
            };

            var res = _CSharpToJavascriptConverter.Map(vm);

            res.ToString().Should().Be("{\"Bool\":true,\"Decimal\":0.01,\"Double\":0.9221,\"Int32\":1}");
        }

        [Fact]
        public void Map_maps_dictionary()
        {
            var vm = new Dictionary<string, object>
            {
                ["integer"] = 1,
                ["string"] = "blablabla",
                ["bool"] = true
            }; 

            var res = _CSharpToJavascriptConverter.Map(vm);

            res.ToString().Should().Be("{\"bool\":true,\"integer\":1,\"string\":\"blablabla\"}");
        }

        [Fact]
        public void Map_maps_expandoObject()
        {
            dynamic vm = new ExpandoObject();
            vm.integer = 1;
            vm.stringValue = "blablabla";
            vm.boolValue = true;

            var res = _CSharpToJavascriptConverter.Map(vm);

            string stringValue = res.ToString();
            stringValue.Should().Be("{\"boolValue\":true,\"integer\":1,\"stringValue\":\"blablabla\"}");
        }

        private class MyDynamicObject : DynamicObject
        {
            public override IEnumerable<string> GetDynamicMemberNames() => new[] {"toto", "nickelback"};

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = binder.Name;
                return true;
            }
        }

        [Fact]
        public void Map_maps_DynamicObject()
        {
            var vm = new MyDynamicObject();
            var res = _CSharpToJavascriptConverter.Map(vm);
            res.ToString().Should().Be("{\"nickelback\":\"nickelback\",\"toto\":\"toto\"}");
        }

        [Fact]
        public void Map_performance_test()
        {
            var converter = GetCSharpToJavascriptConverterForPerformance();
            var vm = SimpleReadOnlyTestViewModel.BuildBigVm();

            using (GetPerformanceCounter("ToJson large Vm"))
            {
                var res = JsonConvert.SerializeObject(vm);
            }

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
            return new CSharpToJavascriptConverter(cacher, factory, _Logger);
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
