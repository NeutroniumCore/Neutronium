using Neutronium.Core.Binding;
using Neutronium.Core.Binding.GlueObject;
using NSubstitute;
using Xunit;
using System.Collections.Generic;
using System.Dynamic;
using System.Windows.Input;
using FluentAssertions;
using MoreCollection.Extensions;
using Neutronium.Core.Binding.GlueBuilder;
using Neutronium.Core.Binding.GlueObject.Basic;
using Neutronium.Core.Binding.GlueObject.Executable;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.Extension;
using Neutronium.Core.Test.Helper;
using Neutronium.MVVMComponents;
using Xunit.Abstractions;
using Newtonsoft.Json;
using Ploeh.AutoFixture.Xunit2;

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
        public void Map_CreateJSGlueObject_WithCorrectToString_WithSingleQuote()
        {
            var testObject = new StringClass
            {
                Value = "Dady's girl"
            };
            var res = _CSharpToJavascriptConverter.Map(testObject);

            res.ToString().Should().Be(@"{""Value"":""Dady's girl""}");
        }

        [Theory]
        [InlineData(double.NaN, "NaN")]
        [InlineData(double.NegativeInfinity, "-Infinity")]
        [InlineData(double.PositiveInfinity, "Infinity")]
        public void Map_CreateJSGlueObject_WithCorrectToString_SpecicalDoubleValue(double value, string json)
        {
            var testObject = new DoubleClass
            {
                Value = value
            };
            var res = _CSharpToJavascriptConverter.Map(testObject);

            res.ToString().Should().Be($@"{{""Value"":{json}}}");
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

        [Theory]
        [InlineData(1L)]
        [InlineData(1F)]
        [InlineData(1)]
        [InlineData(1U)]
        [InlineData(1UL)]
        [InlineData(true)]
        [InlineData("")]
        public void Map_maps_clr_number(object number)
        {
            var res = _CSharpToJavascriptConverter.Map(number);

            var expectedType = typeof(JsBasicTyped<>).MakeGenericType(number.GetType());

            res.GetType().BaseType.Should().Be(expectedType);
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
        public void Map_maps_ISimpleCommand()
        {
            var command = Substitute.For<ISimpleCommand>();

            var res = _CSharpToJavascriptConverter.Map(command);

            res.Should().BeOfType<JsSimpleCommand>();
        }

        [Fact]
        public void Map_maps_gives_priority_to_ISimpleCommand()
        {
            var command = Substitute.For<ISimpleCommand, ICommand>();

            var res = _CSharpToJavascriptConverter.Map(command);

            res.Should().BeOfType<JsSimpleCommand>();
        }

        [Fact]
        public void Map_maps_ISimpleCommand_Generic()
        {
            var command = Substitute.For<ISimpleCommand<string>>();

            var res = _CSharpToJavascriptConverter.Map(command);

            res.Should().BeOfType<JsSimpleCommand<string>>();
        }

        [Fact]
        public void Map_maps_gives_priority_to_ISimpleCommand_generic()
        {
            var command = Substitute.For<ISimpleCommand<string>, ICommand>();

            var res = _CSharpToJavascriptConverter.Map(command);

            res.Should().BeOfType<JsSimpleCommand<string>>();
        }

        [Fact]
        public void Map_maps_ICommand()
        {
            var command = Substitute.For<ICommand>();

            var res = _CSharpToJavascriptConverter.Map(command);

            res.Should().BeOfType<JsCommand>();
        }

        [Fact]
        public void Map_maps_ICommand_Generic()
        {
            var command = Substitute.For<ICommand<string>>();

            var res = _CSharpToJavascriptConverter.Map(command);

            res.Should().BeOfType<JsCommand<string>>();
        }

        [Fact]
        public void Map_maps_gives_priority_to_ICommand_generic()
        {
            var command = Substitute.For<ICommand<string>, ICommand>();

            var res = _CSharpToJavascriptConverter.Map(command);

            res.Should().BeOfType<JsCommand<string>>();
        }

        [Fact]
        public void Map_maps_ICommandWithoutParameter_Generic()
        {
            var command = Substitute.For<ICommandWithoutParameter>();

            var res = _CSharpToJavascriptConverter.Map(command);

            res.Should().BeOfType<JsCommandWithoutParameter>();
        }

        [Fact]
        public void Map_maps_gives_priority_to_ICommandWithoutParameter()
        {
            var command = Substitute.For<ICommandWithoutParameter, ICommand>();

            var res = _CSharpToJavascriptConverter.Map(command);

            res.Should().BeOfType<JsCommandWithoutParameter>();
        }

        [Fact]
        public void Map_maps_IResultCommand()
        {
            var command = Substitute.For<IResultCommand<string>>();

            var res = _CSharpToJavascriptConverter.Map(command);

            res.Should().BeOfType<JsResultCommand<string>>();
        }

        [Fact]
        public void Map_maps_IResultCommand_With_TArg()
        {
            var command = Substitute.For<IResultCommand<string, int>>();

            var res = _CSharpToJavascriptConverter.Map(command);

            res.Should().BeOfType<JsResultCommand<string,int>>();
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

        [Fact]
        public void AsCircularVersionedJson_adds_default_version()
        {
            var vm = new object();
            var res = _CSharpToJavascriptConverter.Map(vm);

            var cjson = res.AsCircularVersionedJson();
            cjson.Should().Be(@"{""version"":2}");
        }

        [Theory, AutoData]
        public void AsCircularVersionedJson_adds_version(int version)
        {
            var vm = new object();
            var res = _CSharpToJavascriptConverter.Map(vm);

            var cjson = res.AsCircularVersionedJson(version);
            cjson.Should().Be($@"{{""version"":{version}}}");
        }

        [Fact]
        public void AsCircularVersionedJson_exports_ISimpleCommand()
        {
            var command = Substitute.For<ISimpleCommand>();
            var vm = new { Command = command };
            var res = _CSharpToJavascriptConverter.Map(vm);

            var cjson = res.AsCircularVersionedJson();
            cjson.Should().Be(@"{""Command"":cmd(true),""version"":2}");
        }

        [Fact]
        public void AsCircularVersionedJson_exports_ISimpleCommand_T()
        {
            var command = Substitute.For<ISimpleCommand<string>>();
            var vm = new { Command = command };
            var res = _CSharpToJavascriptConverter.Map(vm);

            var cjson = res.AsCircularVersionedJson();
            cjson.Should().Be(@"{""Command"":cmd(true),""version"":2}");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AsCircularVersionedJson_exports_ICommand(bool canExecute)
        {
            var command = Substitute.For<ICommand>();
            command.CanExecute(Arg.Any<object>()).Returns(canExecute);
            var vm = new { Command = command };
            var res = _CSharpToJavascriptConverter.Map(vm);

            var cjson = res.AsCircularVersionedJson();
            cjson.Should().Be($@"{{""Command"":cmd({canExecute.ToString().ToLower()}),""version"":2}}");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AsCircularVersionedJson_exports_ICommand_T(bool canExecute)
        {
            var command = Substitute.For<ICommand<int>>();
            command.CanExecute(Arg.Any<int>()).Returns(canExecute);
            var vm = new { Command = command };
            var res = _CSharpToJavascriptConverter.Map(vm);

            var cjson = res.AsCircularVersionedJson();
            cjson.Should().Be($@"{{""Command"":cmd(true),""version"":2}}");
        }

        [Fact]
        public void AsCircularVersionedJson_exports_IResultCommand_T()
        {
            var command = Substitute.For<IResultCommand<int>>();
            var vm = new { Command = command };
            var res = _CSharpToJavascriptConverter.Map(vm);

            var cjson = res.AsCircularVersionedJson();
            cjson.Should().Be($@"{{""Command"":cmd(true),""version"":2}}");
        }

        [Fact]
        public void AsCircularJson_exports_ISimpleCommand()
        {
            var command = Substitute.For<ISimpleCommand>();
            var vm = new { Command = command};
            var res = _CSharpToJavascriptConverter.Map(vm);

            var cjson = res.AsCircularJson();
            cjson.Should().Be(@"{""Command"":cmd(true)}");
        }

        [Fact]
        public void AsCircularJson_exports_ISimpleCommand_T()
        {
            var command = Substitute.For<ISimpleCommand<string>>();
            var vm = new { Command = command };
            var res = _CSharpToJavascriptConverter.Map(vm);

            var cjson = res.AsCircularJson();
            cjson.Should().Be(@"{""Command"":cmd(true)}");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AsCircularJson_exports_ICommand(bool canExecute)
        {
            var command = Substitute.For<ICommand>();
            command.CanExecute(Arg.Any<object>()).Returns(canExecute);
            var vm = new { Command = command };
            var res = _CSharpToJavascriptConverter.Map(vm);

            var cjson = res.AsCircularJson();
            cjson.Should().Be($@"{{""Command"":cmd({canExecute.ToString().ToLower()})}}");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AsCircularJson_exports_ICommand_T(bool canExecute)
        {
            var command = Substitute.For<ICommand<int>>();
            command.CanExecute(Arg.Any<int>()).Returns(canExecute);
            var vm = new { Command = command };
            var res = _CSharpToJavascriptConverter.Map(vm);

            var cjson = res.AsCircularJson();
            cjson.Should().Be($@"{{""Command"":cmd(true)}}");
        }

        [Fact]
        public void AsCircularJson_exports_IResultCommand_T()
        {
            var command = Substitute.For<IResultCommand<int>>();
            var vm = new { Command = command };
            var res = _CSharpToJavascriptConverter.Map(vm);

            var cjson = res.AsCircularJson();
            cjson.Should().Be($@"{{""Command"":cmd(true)}}");
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

        private class DoubleClass
        {
            public double Value { get; set; }
        }
    }
}
