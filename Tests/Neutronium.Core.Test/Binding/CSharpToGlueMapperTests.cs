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
using AutoFixture.Xunit2;
using Neutronium.Core.Binding.Mapper;

namespace Neutronium.Core.Test.Binding
{
    public class CSharpToGlueMapperTests
    {
        private readonly CSharpToGlueMapper _CSharpToGlueMapper;
        private readonly IJavascriptSessionCache _Cacher;
        private readonly IGlueFactory _GlueFactory;
        private readonly IWebSessionLogger _Logger;
        private readonly Dictionary<object, IJsCsGlue> _Cache = new Dictionary<object, IJsCsGlue>();
        private readonly ObjectChangesListener _ObjectChangesListener;
        private readonly ITestOutputHelper _TestOutputHelper;

        private static int CurrentVersion => 3;

        public CSharpToGlueMapperTests(ITestOutputHelper testOutputHelper)
        {
            _TestOutputHelper = testOutputHelper;
            _Cacher = Substitute.For<IJavascriptSessionCache>();
            _Cacher.When(c => c.CacheFromCSharpValue(Arg.Any<object>(), Arg.Any<IJsCsGlue>()))
                   .Do(callInfo => _Cache.Add(callInfo[0], (IJsCsGlue)callInfo[1]));
            _Cacher.GetCached(Arg.Any<object>()).Returns(callInfo => _Cache.GetOrDefault(callInfo[0]));
            _ObjectChangesListener = new ObjectChangesListener(_ => { }, _ => { }, _ => { }, _ => { });
            _GlueFactory = new GlueFactory(null, _Cacher, null, null, _ObjectChangesListener);
            _Logger = Substitute.For<IWebSessionLogger>();
            _CSharpToGlueMapper = new CSharpToGlueMapper(_Cacher, _GlueFactory, _Logger);
        }

        [Fact]
        public void Map_Creates_JSGlueObject_None_Circular_With_Correct_ToString()
        {
            var testObject = new TestClass();
            var res = _CSharpToGlueMapper.Map(testObject);

            res.ToString().Should().Be("{\"Children\":[],\"Property1\":null,\"Property2\":null,\"Property3\":null}");
        }

        [Fact]
        public void Map_Creates_JSGlueObject_nested_With_Correct_ToString()
        {
            var testObject = new TestClass
            {
                Property1 = new TestClass()
            };
            var res = _CSharpToGlueMapper.Map(testObject);

            res.ToString().Should().Be("{\"Children\":[],\"Property1\":{\"Children\":[],\"Property1\":null,\"Property2\":null,\"Property3\":null},\"Property2\":null,\"Property3\":null}");
        }

        [Fact]
        public void Map_Creates_JSGlueObject_With_Circular_Root_With_Correct_ToString()
        {
            var testObject = new TestClass();
            testObject.Property1 = testObject;
            var res = _CSharpToGlueMapper.Map(testObject);

            res.ToString().Should().Be("{\"Children\":[],\"Property1\":\"~\",\"Property2\":null,\"Property3\":null}");
        }

        [Fact]
        public void Map_Creates_JSGlueObject_With_Circular_Property_With_Correct_ToString()
        {
            var testObject = new TestClass();
            var testObject2 = new TestClass();
            testObject.Property1 = testObject2;
            testObject.Property2 = testObject2;
            var res = _CSharpToGlueMapper.Map(testObject);

            res.ToString().Should().Be("{\"Children\":[],\"Property1\":{\"Children\":[],\"Property1\":null,\"Property2\":null,\"Property3\":null},\"Property2\":\"~Property1\",\"Property3\":null}");
        }

        [Fact]
        public void Map_Creates_JSGlueObject_With_Circular_NestedProperty_With_Correct_ToString()
        {
            var testObject = new TestClass();
            var testObject2 = new TestClass();
            testObject.Property1 = new TestClass
            {
                Property2 = testObject2
            };
            testObject.Property3 = testObject2;
            var res = _CSharpToGlueMapper.Map(testObject);

            res.ToString().Should().Be("{\"Children\":[],\"Property1\":{\"Children\":[],\"Property1\":null,\"Property2\":{\"Children\":[],\"Property1\":null,\"Property2\":null,\"Property3\":null},\"Property3\":null},\"Property2\":null,\"Property3\":\"~Property1~Property2\"}");
        }

        [Fact]
        public void Map_Creates_JSGlueObject_With_Circular_ListProperty_With_Correct_ToString()
        {
            var testObject = new TestClass();
            var tesObject2 = new TestClass();
            var children = new List<TestClass> { tesObject2 };
            testObject.Children = children;
            tesObject2.Children = children;
            var res = _CSharpToGlueMapper.Map(testObject);

            res.ToString().Should().Be("{\"Children\":[{\"Children\":\"~Children\",\"Property1\":null,\"Property2\":null,\"Property3\":null}],\"Property1\":null,\"Property2\":null,\"Property3\":null}");
        }

        [Fact]
        public void Map_Creates_JSGlueObject_Simple_List_With_Correct_ToString()
        {
            var testObject = new TestClass();
            testObject.Children.Add(testObject);
            var res = _CSharpToGlueMapper.Map(testObject);

            res.ToString().Should().Be("{\"Children\":[\"~\"],\"Property1\":null,\"Property2\":null,\"Property3\":null}");
        }

        [Fact]
        public void Map_Creates_JSGlueObject_With_Correct_ToString_TransformingQuote()
        {
            var testObject = new StringClass
            {
                Value = @"a""quote"""
            };
            var res = _CSharpToGlueMapper.Map(testObject);

            res.ToString().Should().Be(@"{""Value"":""a\""quote\""""}");
        }

        [Fact]
        public void Map_Creates_JSGlueObject_With_Correct_ToString_TransformingSlash()
        {
            var testObject = new StringClass
            {
                Value = @"C:\Users\David\Documents\Source\DiscogsClient\DiscogsClient\bin\Debug\DiscogsClient.dll"
            };
            var res = _CSharpToGlueMapper.Map(testObject);

            res.ToString().Should().Be(@"{""Value"":""C:\\Users\\David\\Documents\\Source\\DiscogsClient\\DiscogsClient\\bin\\Debug\\DiscogsClient.dll""}");
        }

        [Fact]
        public void Map_Creates_JSGlueObject_With_Single_Quote_String_With_Correct_ToString()
        {
            var testObject = new StringClass
            {
                Value = "Dady's girl"
            };
            var res = _CSharpToGlueMapper.Map(testObject);

            res.ToString().Should().Be(@"{""Value"":""Dady's girl""}");
        }

        [Theory]
        [InlineData('\\', "\\\\")]
        [InlineData('\n', "\\n")]
        [InlineData('\r', "\\r")]
        [InlineData('\t', "\\t")]
        [InlineData('\f', "\\f")]
        [InlineData('\b', "\\b")]
        public void Map_Creates_JSGlueObject_With_Char_With_Correct_ToString(char value, string expected)
        {
            var testObject = new { Value = value};
            var res = _CSharpToGlueMapper.Map(testObject);

            res.ToString().Should().Be($@"{{""Value"":""{expected}""}}");
        }

        [Theory]
        [InlineData(double.NaN, "NaN")]
        [InlineData(double.NegativeInfinity, "-Infinity")]
        [InlineData(double.PositiveInfinity, "Infinity")]
        public void Map_Creates_JSGlueObject_With_Special_Double_value_With_Correct_ToString(double value, string json)
        {
            var testObject = new DoubleClass
            {
                Value = value
            };
            var res = _CSharpToGlueMapper.Map(testObject);

            res.ToString().Should().Be($@"{{""Value"":{json}}}");
        }

        [Fact]
        public void Map_Creates_JSGlueObject_With_List_Property_With_Correct_ToString()
        {
            var testObject = new TestClass();
            var testObject2 = new TestClass();
            testObject.Property3 = testObject2;
            testObject.Children.Add(testObject2);
            var res = _CSharpToGlueMapper.Map(testObject);

            res.ToString().Should().Be("{\"Children\":[{\"Children\":[],\"Property1\":null,\"Property2\":null,\"Property3\":null}],\"Property1\":null,\"Property2\":null,\"Property3\":\"~Children~0\"}");
        }

        [Theory]
        [InlineData(1L)]
        [InlineData(1F)]
        [InlineData(1)]
        [InlineData(1U)]
        [InlineData(1UL)]
        [InlineData(true)]
        public void Map_Maps_Clr_Number(object number)
        {
            var res = _CSharpToGlueMapper.Map(number);

            var expectedType = typeof(JsBasicTyped<>).MakeGenericType(number.GetType());

            res.GetType().BaseType.Should().Be(expectedType);
        }

        [Theory]
        [InlineData("")]
        [InlineData("abcd")]
        public void Map_Maps_String(string stringValue)
        {
            var res = _CSharpToGlueMapper.Map(stringValue);

            var expectedType = typeof(JsBasicGarbageCollectedTyped<string>);

            res.GetType().BaseType.Should().Be(expectedType);
        }

        [Fact]
        public void Map_Maps_Nullable_Value()
        {
            var vm = new NullableTestViewModel
            {
                Bool = true,
                Decimal = 0.01m,
                Double = 0.9221,
                Int32 = 1
            };

            var res = _CSharpToGlueMapper.Map(vm);

            res.ToString().Should().Be("{\"Bool\":true,\"Decimal\":0.01,\"Double\":0.9221,\"Int32\":1}");
        }

        [Fact]
        public void Map_Maps_ISimpleCommand()
        {
            var command = Substitute.For<ISimpleCommand>();

            var res = _CSharpToGlueMapper.Map(command);

            res.Should().BeOfType<JsSimpleCommand>();
        }

        [Fact]
        public void Map_Gives_Priority_To_ISimpleCommand()
        {
            var command = Substitute.For<ISimpleCommand, ICommand>();

            var res = _CSharpToGlueMapper.Map(command);

            res.Should().BeOfType<JsSimpleCommand>();
        }

        [Fact]
        public void Map_Maps_ISimpleCommand_Generic()
        {
            var command = Substitute.For<ISimpleCommand<string>>();

            var res = _CSharpToGlueMapper.Map(command);

            res.Should().BeOfType<JsSimpleCommand<string>>();
        }

        [Fact]
        public void Map_Gives_Priority_To_ISimpleCommand_Generic()
        {
            var command = Substitute.For<ISimpleCommand<string>, ICommand>();

            var res = _CSharpToGlueMapper.Map(command);

            res.Should().BeOfType<JsSimpleCommand<string>>();
        }

        [Fact]
        public void Map_Maps_ICommand()
        {
            var command = Substitute.For<ICommand>();

            var res = _CSharpToGlueMapper.Map(command);

            res.Should().BeOfType<JsCommand>();
        }

        [Fact]
        public void Map_Maps_ICommand_Generic()
        {
            var command = Substitute.For<ICommand<string>>();

            var res = _CSharpToGlueMapper.Map(command);

            res.Should().BeOfType<JsCommand<string>>();
        }

        [Fact]
        public void Map_Gives_Priority_To_ICommand_Generic()
        {
            var command = Substitute.For<ICommand<string>, ICommand>();

            var res = _CSharpToGlueMapper.Map(command);

            res.Should().BeOfType<JsCommand<string>>();
        }

        [Fact]
        public void Map_Maps_ICommandWithoutParameter_Generic()
        {
            var command = Substitute.For<ICommandWithoutParameter>();

            var res = _CSharpToGlueMapper.Map(command);

            res.Should().BeOfType<JsCommandWithoutParameter>();
        }

        [Fact]
        public void Map_Gives_Priority_To_ICommandWithoutParameter()
        {
            var command = Substitute.For<ICommandWithoutParameter, ICommand>();

            var res = _CSharpToGlueMapper.Map(command);

            res.Should().BeOfType<JsCommandWithoutParameter>();
        }

        [Fact]
        public void Map_Maps_IResultCommand()
        {
            var command = Substitute.For<IResultCommand<string>>();

            var res = _CSharpToGlueMapper.Map(command);

            res.Should().BeOfType<JsResultCommand<string>>();
        }

        [Fact]
        public void Map_Maps_IResultCommand_With_TArg()
        {
            var command = Substitute.For<IResultCommand<string, int>>();

            var res = _CSharpToGlueMapper.Map(command);

            res.Should().BeOfType<JsResultCommand<string,int>>();
        }

        [Fact]
        public void Map_Maps_Dictionary()
        {
            var vm = new Dictionary<string, object>
            {
                ["integer"] = 1,
                ["string"] = "blablabla",
                ["bool"] = true
            }; 

            var res = _CSharpToGlueMapper.Map(vm);

            res.ToString().Should().Be("{\"bool\":true,\"integer\":1,\"string\":\"blablabla\"}");
        }

        [Fact]
        public void Map_Maps_ExpandoObject()
        {
            dynamic vm = new ExpandoObject();
            vm.integer = 1;
            vm.stringValue = "blablabla";
            vm.boolValue = true;

            var res = _CSharpToGlueMapper.Map(vm);

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
        public void Map_Maps_DynamicObject()
        {
            var vm = new MyDynamicObject();
            var res = _CSharpToGlueMapper.Map(vm);
            res.ToString().Should().Be("{\"nickelback\":\"nickelback\",\"toto\":\"toto\"}");
        }

        [Fact]
        public void Map_Performance_Test()
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
        public void AsCircularVersionedJson_Adds_Default_Version()
        {
            var vm = new object();
            var res = _CSharpToGlueMapper.Map(vm);

            var cJson = res.AsCircularVersionedJson();
            cJson.Should().Be($@"{{""version"":{CurrentVersion}}}");
        }

        [Theory, AutoData]
        public void AsCircularVersionedJson_Adds_Version(int version)
        {
            var vm = new object();
            var res = _CSharpToGlueMapper.Map(vm);

            var cJson = res.AsCircularVersionedJson(version);
            cJson.Should().Be($@"{{""version"":{version}}}");
        }

        [Fact]
        public void AsCircularVersionedJson_Exports_ISimpleCommand()
        {
            var command = Substitute.For<ISimpleCommand>();
            var vm = new { Command = command };
            var res = _CSharpToGlueMapper.Map(vm);

            var cJson = res.AsCircularVersionedJson();
            cJson.Should().Be($@"{{""Command"":cmd(true),""version"":{CurrentVersion}}}");
        }

        [Fact]
        public void AsCircularVersionedJson_Exports_ISimpleCommand_T()
        {
            var command = Substitute.For<ISimpleCommand<string>>();
            var vm = new { Command = command };
            var res = _CSharpToGlueMapper.Map(vm);

            var cJson = res.AsCircularVersionedJson();
            cJson.Should().Be($@"{{""Command"":cmd(true),""version"":{CurrentVersion}}}");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AsCircularVersionedJson_Exports_ICommand(bool canExecute)
        {
            var command = Substitute.For<ICommand>();
            command.CanExecute(Arg.Any<object>()).Returns(canExecute);
            var vm = new { Command = command };
            var res = _CSharpToGlueMapper.Map(vm);

            var cJson = res.AsCircularVersionedJson();
            cJson.Should().Be($@"{{""Command"":cmd({canExecute.ToString().ToLower()}),""version"":{CurrentVersion}}}");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AsCircularVersionedJson_Exports_ICommand_T(bool canExecute)
        {
            var command = Substitute.For<ICommand<int>>();
            command.CanExecute(Arg.Any<int>()).Returns(canExecute);
            var vm = new { Command = command };
            var res = _CSharpToGlueMapper.Map(vm);

            var cJson = res.AsCircularVersionedJson();
            cJson.Should().Be($@"{{""Command"":cmd(true),""version"":{CurrentVersion}}}");
        }

        [Fact]
        public void AsCircularVersionedJson_Exports_IResultCommand_T()
        {
            var command = Substitute.For<IResultCommand<int>>();
            var vm = new { Command = command };
            var res = _CSharpToGlueMapper.Map(vm);

            var cJson = res.AsCircularVersionedJson();
            cJson.Should().Be($@"{{""Command"":cmd(true),""version"":{CurrentVersion}}}");
        }

        [Fact]
        public void AsCircularJson_Exports_ISimpleCommand()
        {
            var command = Substitute.For<ISimpleCommand>();
            var vm = new { Command = command};
            var res = _CSharpToGlueMapper.Map(vm);

            var cJson = res.AsCircularJson();
            cJson.Should().Be(@"{""Command"":cmd(true)}");
        }

        [Fact]
        public void AsCircularJson_Exports_ISimpleCommand_T()
        {
            var command = Substitute.For<ISimpleCommand<string>>();
            var vm = new { Command = command };
            var res = _CSharpToGlueMapper.Map(vm);

            var cJson = res.AsCircularJson();
            cJson.Should().Be(@"{""Command"":cmd(true)}");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AsCircularJson_Exports_ICommand(bool canExecute)
        {
            var command = Substitute.For<ICommand>();
            command.CanExecute(Arg.Any<object>()).Returns(canExecute);
            var vm = new { Command = command };
            var res = _CSharpToGlueMapper.Map(vm);

            var cJson = res.AsCircularJson();
            cJson.Should().Be($@"{{""Command"":cmd({canExecute.ToString().ToLower()})}}");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AsCircularJson_Exports_ICommand_T(bool canExecute)
        {
            var command = Substitute.For<ICommand<int>>();
            command.CanExecute(Arg.Any<int>()).Returns(canExecute);
            var vm = new { Command = command };
            var res = _CSharpToGlueMapper.Map(vm);

            var cJson = res.AsCircularJson();
            cJson.Should().Be($@"{{""Command"":cmd(true)}}");
        }

        [Fact]
        public void AsCircularJson_Exports_IResultCommand_T()
        {
            var command = Substitute.For<IResultCommand<int>>();
            var vm = new { Command = command };
            var res = _CSharpToGlueMapper.Map(vm);

            var cJson = res.AsCircularJson();
            cJson.Should().Be($@"{{""Command"":cmd(true)}}");
        }

        protected PerformanceHelper GetPerformanceCounter(string description) => new PerformanceHelper(_TestOutputHelper, description);

        private CSharpToGlueMapper GetCSharpToJavascriptConverterForPerformance()
        {
            var cacher = new SessionCacher();
            var factory = new GlueFactory(null, cacher, null, null, _ObjectChangesListener);
            return new CSharpToGlueMapper(cacher, factory, _Logger);
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
