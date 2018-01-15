using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using FluentAssertions;
using Neutronium.Core.Test.Helper;
using Neutronium.Core.Utils;
using Neutronium.MVVMComponents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NSubstitute;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Neutronium.Core.Test.Utils
{
    public class CJsonConverterTests
    {
        private readonly CJsonConverter _CJsonConverter = new CJsonConverter();

        [Fact]
        public void ToRootVmCjson_adds_vm_information()
        {
            var vm = new object();
            CheckToRootVmCjson(vm, "{}");
        }

        public static IEnumerable<object> SimpleCommands
        {
            get
            {
                yield return Substitute.For<ISimpleCommand>();
                yield return Substitute.For<ISimpleCommand<string>>();
                yield return Substitute.For<ISimpleCommand<int>>();
            }
        }

        public static IEnumerable<object[]> SimpleCommandsData 
            => SimpleCommands.Select(c => new[] { c, GetExpectedCommand(true)});

        public static IEnumerable<object[]> CommandsData
        {
            get
            {
                var expectedTrue = GetExpectedCommand(true);
                foreach (var canExecute in Enumerable.Range(0,2).Select(i => i==0))
                {
                    var command = Substitute.For<ICommand>();
                    command.CanExecute(Arg.Any<object>()).Returns(canExecute);
                    yield return new object[] {command, GetExpectedCommand(canExecute)};

                    var commandString = Substitute.For<ICommand<int>>();
                    commandString.CanExecute(Arg.Any<int>()).Returns(canExecute);
                    yield return new object[] { commandString, expectedTrue };
                }

                yield return new object[] { Substitute.For<IResultCommand<int>>(), expectedTrue };
            }
        }

        [Theory]
        [MemberData(nameof(SimpleCommandsData))]
        public void ToRootVmCjson_exports_ISimpleCommand(object simpleCommand, string expected)
        {
            var vm = new { Command = simpleCommand };
            CheckToRootVmCjson(vm, expected);
        }

        [Theory]
        [MemberData(nameof(CommandsData))]
        public void ToRootVmCjson_exports_Commands(object command, string expected)
        {
            var vm = new { Command = command };
            CheckToRootVmCjson(vm, expected);
        }

        [Theory, AutoData]
        public void ToRootVmCjson_exports_object_including_all_clr_types(ClrTypesTestViewModel vm)
        {
            var expected = ToJson(vm);
            CheckToRootVmCjson(vm, expected);
        }

        [Theory, AutoData]
        public void ToRootVmCjson_exports_object_including_string(StringViewModel vm)
        {
            var expected = ToJson(vm);
            CheckToRootVmCjson(vm, expected);
        }

        [Theory, AutoData]
        public void ToRootVmCjson_exports_date_time(DateTime dateTime)
        {
            var vm = new { dateTime };
            var expected = $@"{{""dateTime"":d({ToJson(dateTime)})}}";
            CheckToRootVmCjson(vm, expected);
        }

        [Theory]
        [MemberData(nameof(SimpleCommandsData))]
        public void ToCjson_exports_ISimpleCommand(object simpleCommand, string expected)
        {
            var vm = new { Command = simpleCommand };
            CheckToCjson(vm, expected);
        }

        [Theory]
        [MemberData(nameof(CommandsData))]
        public void ToCjson_exports_Commands(object command, string expected)
        {
            var vm = new { Command = command };
            CheckToCjson(vm, expected);
        }

        [Theory, AutoData]
        public void ToCjson_exports_object_including_all_clr_types(ClrTypesTestViewModel vm)
        {
            var expected = ToJson(vm);
            CheckToCjson(vm, expected);
        }

        [Theory, AutoData]
        public void ToCjson_exports_object_including_string(StringViewModel vm)
        {
            var expected = ToJson(vm);
            CheckToCjson(vm, expected);
        }

        [Theory, AutoData]
        public void ToCjson_exports_date_time(DateTime dateTime)
        {
            var vm = new { dateTime };
            var expected = $@"{{""dateTime"":d({ToJson(dateTime)})}}";
            CheckToCjson(vm, expected);
        }

        private void CheckToCjson(object vm, string expectedVm)
        {
            var cjson = _CJsonConverter.ToCjson(vm);
            cjson.Should().Be(expectedVm);
        }

        private void CheckToRootVmCjson(object vm, string expectedVm)
        {
            var cjson = _CJsonConverter.ToRootVmCjson(vm);
            cjson.Should().Be(GetExpectedRootVm(expectedVm));
        }

        private static string GetExpectedCommand(bool canExecute) 
            => $@"{{""Command"":cmd({canExecute.ToString().ToLower()})}}";

        private static string GetExpectedRootVm(string @object = "{}", int version = 3)
        {
            return "{\"ViewModel\":" + @object + ",\"Window\":{\"CloseReady\":cmd(true),\"EndOpen\":cmd(true),\"IsListeningClose\":false,\"IsListeningOpen\":false,\"State\":{\"type\":\"WindowLogicalState\",\"intValue\":0,\"name\":\"Loading\",\"displayName\":\"Loading\"}},\"version\":" + version + "}";
        }

        private static string ToJson(object @object)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new OrderedContractResolver(),
                Converters = new List<JsonConverter> { new NumericConverter<float>(), new NumericConverter<decimal>(), new NumericConverter<double>() }
            };
            return JsonConvert.SerializeObject(@object, Formatting.None, settings);
        }

        private class OrderedContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(System.Type type, MemberSerialization memberSerialization)
            {
                return base.CreateProperties(type, memberSerialization).OrderBy(p => p.PropertyName).ToList();
            }
        }

        private class NumericConverter<T> : JsonConverter where T : struct
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(T) || objectType == typeof(T?));
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                if (value == null)
                {
                    writer.WriteNull();
                    return;
                }
                var dd = Convert.ToDecimal(value);
                if (decimal.Ceiling(dd) == dd)
                {
                    JToken.FromObject((int)dd).WriteTo(writer);
                    return;
                }
                var d = (T)value;
                JToken.FromObject(d).WriteTo(writer);
            }
        }
    }
}
