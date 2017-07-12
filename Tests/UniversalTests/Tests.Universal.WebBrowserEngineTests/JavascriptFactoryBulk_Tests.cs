using System;
using FluentAssertions;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Tests.Infra.WebBrowserEngineTesterHelper.Context;
using Tests.Infra.WebBrowserEngineTesterHelper.Windowless;
using Xunit;
using Xunit.Abstractions;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;

namespace Tests.Universal.WebBrowserEngineTests
{
    public abstract class JavascriptFactoryBulk_Tests : TestBase
    {
        protected JavascriptFactoryBulk_Tests(IBasicWindowLessHTMLEngineProvider testEnvironment, ITestOutputHelper output)
                        : base(testEnvironment, output)
        {
        }

        private IJavascriptObject Get(object @object)
        {
            return Factory.CreateBasics(new[] { @object }).Single();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        [InlineData(10)]
        [InlineData("tititi")]
        [InlineData(0.5D)]
        [InlineData(-1)]
        [InlineData(99999.95)]
        public void Test_GetSimpleValue(object value)
        {
            Test(() =>
               {
                   object res = null;
                   bool ok = Converter.GetSimpleValue(Get(value), out res);
                   ok.Should().BeTrue();
                   res.Should().Be(value);
               });
        }

        [Property]
        public Property CreateBasics_Returns_Correct_Value_Int()
        {
            return CreateBasics_Returns_Correct_Value<int>();
        }

        public Property CreateBasics_Returns_Correct_Value<T>()
        {
            return Prop.ForAll<T>(value =>
                Test(() =>
                {
                    var res = ConvertBack(value);
                    return Object.Equals(res,value);
                })
            );
        }

        private T ConvertBack<T>(T value)
        {
            object res = null;
            Converter.GetSimpleValue(Get(value), out res);
            return (T)res;
        }


        [Fact]
        public void Test_GetSimpleValue_Date()
        {
            Test(() =>
            {
                object res = null;
                var date = new DateTime(1974, 02, 26, 01, 02, 03, DateTimeKind.Utc);
                var dateJavascript = Get(date);
                var ok = Converter.GetSimpleValue(dateJavascript, out res);
                ok.Should().BeTrue();
                res.Should().Be(date);
            });
        }

        [Fact]
        public void Test_GetSimpleValue_Uint_explicit()
        {
            Test(() =>
            {
                object res = null;
                IJavascriptObject maxuint = Get(uint.MaxValue);
                var ok = Converter.GetSimpleValue(maxuint, out res, typeof(UInt32));
                ok.Should().BeTrue();
                res.Should().Be(uint.MaxValue);
            });
        }
    }
}
