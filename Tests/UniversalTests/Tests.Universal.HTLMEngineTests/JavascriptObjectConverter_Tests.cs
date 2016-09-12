using System;
using FluentAssertions;
using Neutronium.Core.JavascriptEngine.JavascriptObject;
using Tests.Infra.HTMLEngineTesterHelper.Context;
using Tests.Infra.HTMLEngineTesterHelper.Windowless;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Universal.HTMLEngineTests
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
                    object res = null;
                    bool ok = Converter.GetSimpleValue(Factory.CreateString("titi"), out res);
                    ok.Should().BeTrue();
                    res.Should().Be("titi");
                });
        }

        [Fact]
        public void Test_GetSimpleValue_Int()
        {
            Test(() =>
               {
                   object res = null;
                   bool ok = Converter.GetSimpleValue(Factory.CreateInt(10), out res);
                   ok.Should().BeTrue();
                   res.Should().Be(10);
               });
        }

        [Fact]
        public void Test_GetSimpleValue_Bool_False()
        {
            Test(() =>
               {
                   object res = null;
                   bool ok = Converter.GetSimpleValue(Factory.CreateBool(false), out res);
                   ok.Should().BeTrue();
                   res.Should().Be(false);
               });
        }

        [Fact]
        public void Test_GetSimpleValue_Bool_True()
        {
            Test(() =>
               {
                   object res = null;
                   bool ok = Converter.GetSimpleValue(Factory.CreateBool(true), out res);
                   ok.Should().BeTrue();
                   res.Should().Be(true);
               });
        }

        [Fact]
        public void Test_GetSimpleValue_Double()
        {
            Test(() =>
               {
                   object res = null;
                   bool ok = Converter.GetSimpleValue(Factory.CreateDouble(0.5), out res);
                   ok.Should().BeTrue();
                   res.Should().Be(0.5D);
               });
        }

        [Fact]
        public void Test_GetSimpleValue_Undefined()
        {
            Test(() =>
              {
                  object res = null;
                  bool ok = Converter.GetSimpleValue(Factory.CreateUndefined(), out res);
                  ok.Should().BeTrue();
                  res.Should().Be(null);
              });
        }

        [Fact]
        public void Test_GetSimpleValue_Date()
        {
            Test(() =>
            {
                object res = null;
                var date = new DateTime(1974, 02, 26, 01, 02, 03, DateTimeKind.Utc);
                IJavascriptObject dateJavascript = null;
                bool ok = Factory.CreateBasic(date, out dateJavascript);
                ok.Should().BeTrue();
                ok = Converter.GetSimpleValue(dateJavascript, out res);
                ok.Should().BeTrue();
                res.Should().Be(date);
            });
        }

        [Fact]
        public void Test_GetSimpleValue_Null()
        {
            Test(() =>
            {
                object res = null;
                bool ok = Converter.GetSimpleValue(Factory.CreateNull(), out res);
                ok.Should().BeTrue();
                res.Should().BeNull();
            });
        }

        [Fact]
        public void Test_GetSimpleValue_Uint_explicit()
        {
            Test(() =>
            {
                object res = null;
                IJavascriptObject maxuint = null;
                bool ok = Factory.CreateBasic(uint.MaxValue, out maxuint);
                ok.Should().BeTrue();
                ok = Converter.GetSimpleValue(maxuint, out res, typeof(UInt32));
                ok.Should().BeTrue();
                res.Should().Be(uint.MaxValue);
            });
        }

        [Fact]
        public void Test_GetSimpleValue_int_priority()
        {
            Test(() =>
            {
                object res = null;
                bool ok = Converter.GetSimpleValue(Factory.CreateInt(-1), out res);
                ok.Should().BeTrue();
                res.Should().Be(-1);
            });
        }
    }
}
