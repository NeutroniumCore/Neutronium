using System;
using FluentAssertions;
using Neutronium.Core.Infra.Reflection;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Tests.Infra.WebBrowserEngineTesterHelper.Context;
using Tests.Infra.WebBrowserEngineTesterHelper.Windowless;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Universal.WebBrowserEngineTests
{
    public abstract class JavascriptObjectConverter_Tests : TestBase
    {
        protected JavascriptObjectConverter_Tests(IBasicWindowLessHTMLEngineProvider testEnvironment, ITestOutputHelper output)
                        : base(testEnvironment, output)
        {
        }

        [Fact]
        public void Test_GetSimpleValue_String()
        {
            Test(() =>
            {
                var ok = Converter.GetSimpleValue(Factory.CreateString("titi"), out var res);
                ok.Should().BeTrue();
                res.Should().Be("titi");
            });
        }

        [Fact]
        public void Test_GetSimpleValue_Int()
        {
            Test(() =>
            {
                var ok = Converter.GetSimpleValue(Factory.CreateInt(10), out var res);
                ok.Should().BeTrue();
                res.Should().Be(10);
            });
        }

        [Fact]
        public void Test_GetSimpleValue_Bool_False()
        {
            Test(() =>
            {
                var ok = Converter.GetSimpleValue(Factory.CreateBool(false), out var res);
                ok.Should().BeTrue();
                res.Should().Be(false);
            });
        }

        [Fact]
        public void Test_GetSimpleValue_Bool_True()
        {
            Test(() =>
            {
                var ok = Converter.GetSimpleValue(Factory.CreateBool(true), out var res);
                ok.Should().BeTrue();
                res.Should().Be(true);
            });
        }

        [Fact]
        public void Test_GetSimpleValue_Double()
        {
            Test(() =>
            {
                var ok = Converter.GetSimpleValue(Factory.CreateDouble(0.5), out var res);
                ok.Should().BeTrue();
                res.Should().Be(0.5D);
            });
        }

        [Fact]
        public void Test_GetSimpleValue_Undefined()
        {
            Test(() =>
            {
                var ok = Converter.GetSimpleValue(Factory.CreateUndefined(), out var res);
                ok.Should().BeTrue();
                res.Should().Be(null);
            });
        }

        [Fact]
        public void Test_GetSimpleValue_Date()
        {
            Test(() =>
            {
                var date = new DateTime(1974, 02, 26, 01, 02, 03, DateTimeKind.Utc);
                var dateJavascript = Factory.CreateDateTime(date);
                var ok = Converter.GetSimpleValue(dateJavascript, out var res);
                ok.Should().BeTrue();
                res.Should().Be(date);
            });
        }

        [Fact]
        public void Test_GetSimpleValue_Null()
        {
            Test(() =>
            {
                bool ok = Converter.GetSimpleValue(Factory.CreateNull(), out var res);
                ok.Should().BeTrue();
                res.Should().BeNull();
            });
        }

        [Fact]
        public void Test_GetSimpleValue_Uint_explicit()
        {
            Test(() =>
            {
                var maxuint = Factory.CreateUint(uint.MaxValue);
                var ok = Converter.GetSimpleValue(maxuint, out var res, typeof(UInt32));
                ok.Should().BeTrue();
                res.Should().Be(uint.MaxValue);
            });
        }

        [Theory]
        [InlineData(ObjectObservability.None)]
        [InlineData(ObjectObservability.ReadOnly)]
        [InlineData(ObjectObservability.Observable)]
        [InlineData(ObjectObservability.ReadOnlyObservable)]
        public void Test_factory_CreateObject_create_object_with_correct_flag(ObjectObservability objectObservability)
        {
            Test(() =>
            {
                var @object = Factory.CreateObject(objectObservability);
                var flag = (ObjectObservability) @object.GetValue(NeutroniumConstants.ReadOnlyFlag).GetIntValue();
                flag.Should().Be(objectObservability);
            });
        }

        [Fact]
        public void Test_GetSimpleValue_int_priority()
        {
            Test(() =>
            {
                var ok = Converter.GetSimpleValue(Factory.CreateInt(-1), out var res);
                ok.Should().BeTrue();
                res.Should().Be(-1);
            });
        }
    }
}
