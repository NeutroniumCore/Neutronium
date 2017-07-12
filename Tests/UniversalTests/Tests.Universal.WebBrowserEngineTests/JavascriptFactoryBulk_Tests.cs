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
using System.Collections.Generic;

namespace Tests.Universal.WebBrowserEngineTests
{
    public abstract class JavascriptFactoryBulk_Tests : TestBase
    {
        protected JavascriptFactoryBulk_Tests(IBasicWindowLessHTMLEngineProvider testEnvironment, ITestOutputHelper output)
                        : base(testEnvironment, output)
        {
            Arb.Register<DateTimeGenerator>();
        }

        public class DateTimeGenerator
        {
            public static Arbitrary<DateTime> ArbitraryDateTime()
            {
                //var generator = Gen.zip3(Gen.Choose(0, 2020 * 365 * 24), Gen.Choose(0, 3599), Gen.Choose(0, 9999))
                //                            .Select(t => new DateTime(t.Item1 * TimeSpan.TicksPerHour).AddSeconds(t.Item2).AddMilliseconds(t.Item3));

                var generator = Gen.zip3(Gen.Choose(1800, 2020), Gen.Choose(0, 365), Gen.zip(Gen.Choose(0, 24), Gen.Choose(0, 3600)))
                                          .Select(t => new DateTime(t.Item1, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(t.Item2).AddHours(t.Item3.Item1).AddSeconds(t.Item3.Item2));


                return Arb.From(generator);
            }
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
        [InlineData("")]
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
                   res.Should().Equals(value);
               });
        }

        [Fact]
        public void Test_GetSimpleValue_Decimal()
        {
            decimal value = 7.9228162495817593524129366018M;
            Test(() =>
            {
                var res = ConvertBack(value);
                res.Should().Equals(value);
            });
        }

        [Property]
        public Property CreateBasics_Returns_Correct_Value_Int()
        {
            return CreateBasics_Returns_Correct_Value<int>();
        }

        //[Property]
        //public Property CreateBasics_Returns_Correct_Value_Decimal()
        //{
        //    return CreateBasics_Returns_Correct_Value<decimal>();
        //}

        [Property]
        public Property CreateBasics_Returns_Correct_Value_Double()
        {
            return CreateBasics_Returns_Correct_Value<double>();
        }

        [Property]
        public Property CreateBasics_Returns_Correct_Value_String()
        {
            return CreateBasics_Returns_Correct_Value<string>();
        }

        [Property]
        public Property CreateBasics_Returns_Correct_Value_DateTime()
        {
            return CreateBasics_Returns_Correct_Value<DateTime>();
        }

        public Property CreateBasics_Returns_Correct_Value<T>()
        {
            return Prop.ForAll<T>(value =>
                Test(() =>
                {
                    var res = ConvertBack(value);
                    return EqualityComparer<T>.Default.Equals(res, value);
                })
            );
        }

        private T ConvertBack<T>(T value)
        {
            if (value == null)
                return default(T);

            object res = null;
            Converter.GetSimpleValue(Get(value), out res, typeof(T));
            return (T)res;
        }

        public static IEnumerable<object> DateTimes 
        {
            get 
            {
                yield return new object[] { new DateTime(1974, 02, 26, 01, 02, 03, DateTimeKind.Utc) };
                yield return new object[] { new DateTime(1967, 01, 23, 0, 0, 0, DateTimeKind.Utc)};
                yield return new object[] { new DateTime(1979, 04, 03, 18, 59, 37, DateTimeKind.Utc)};
                yield return new object[] { new DateTime(1941, 09, 04, 19, 05, 31, DateTimeKind.Utc)};
                yield return new object[] { new DateTime(1824, 11, 10, 03, 47, 27, DateTimeKind.Utc)};
            }
        }

        [Theory]
        [MemberData(nameof(DateTimes))]
        public void Test_GetSimpleValue_Date(DateTime date)
        {
            Test(() =>
            {
                object res = null;
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
