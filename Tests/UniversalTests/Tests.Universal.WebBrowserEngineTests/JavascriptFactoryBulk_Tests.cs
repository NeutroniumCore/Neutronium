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
            Arb.Register<TestDataGenerator>();
        }

        protected abstract bool SupportStringEmpty { get; }

        public class TestDataGenerator
        {
            private static Gen<DateTime> DateTimeGenerator => 
                Gen.zip3(Gen.Choose(1800, 2020), Gen.Choose(0, 365), Gen.zip(Gen.Choose(0, 24), Gen.Choose(0, 3600)))
                   .Select(t => new DateTime(t.Item1, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(t.Item2).AddHours(t.Item3.Item1).AddSeconds(t.Item3.Item2));

            private static Gen<T> DefaultGenerator<T>() => Arb.From<T>().Generator;

            private static Gen<object> DefaultObjectGenerator<T>() => DefaultGenerator<T>().Select(t => (object)t);

            private static Gen<object> ObjectGenerator 
                => Gen.OneOf(DefaultObjectGenerator<Boolean>(), DefaultObjectGenerator<int>(), DefaultObjectGenerator<double>(),
                                    DefaultGenerator<string>().Where(s => s != "" && s != null).Select(t => (object)t),
                                    DateTimeGenerator.Select(t => (object)t));

            public static Arbitrary<DateTime> ArbitraryDateTime() => Arb.From(DateTimeGenerator);

            public static Arbitrary<object> ArbitraryObject() => Arb.From(ObjectGenerator);
        }

        private IJavascriptObject Get(object @object)
        {
            return Factory.CreateBasics(new[] { @object }).Single();
        }

        private List<object> GetBack(List<object> @objects)
        {
            return Test(() =>
            {
                var jvos = Factory.CreateBasics(@objects);
                return jvos.Select((jvo, i) => ConvertBack(jvo, @objects[i].GetType())).ToList();
            });
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        [InlineData(10)]
        [InlineData("tititi")]
        [InlineData(0.5D)]
        [InlineData(-1)]
        [InlineData(99999.95)]
        [InlineData(2.333333333)]
        [InlineData(-0.66666666666666663)]
        [InlineData(1.3333333333333333)]
        [InlineData(4.94065645841247E-3240)]
        public void CreateBasics_BasicTypes_Returns_Correct_Value(object value)
        {
            TestConvertion(value);
        }

        [Fact]
        public void CreateBasics_EmptyString_Returns_Correct_Value()
        {
            if (!SupportStringEmpty)
                return;

            TestBasicConvertion(string.Empty);
        }

        [Fact]
        public void CreateBasics_Double_Epsilon_Returns_Correct_Value()
        {
            var epsilon = Double.Epsilon;
            TestBasicConvertion(epsilon);
        }

        private void TestBasicConvertion<T>(T value)
        {
            Test(() =>
            {
                object res = null;
                bool ok = Converter.GetSimpleValue(Get(value), out res, typeof(T));
                ok.Should().BeTrue();
                res.Should().Be(value);
            });
        }

        private void TestConvertion(object value)
        {
            Test(() =>
            {
                object res = null;
                bool ok = Converter.GetSimpleValue(Get(value), out res, value.GetType());
                ok.Should().BeTrue();
                res.Should().NotBeNull();
                res.Should().Equals(value);
            });
        }

        [Fact]
        public void CreateBasics_Decimal_Create_Correct_Objects_Fixed_Value()
        {
            decimal value = 7.9228162495817593524129366018M;
            Test(() =>
            {
                var res = ConvertBack(value);
                res.Should().Equals(value);
            });
        }

        [Property]
        public Property CreateBasics_Int_Create_Correct_Objects()
        {
            return CreateBasics_Returns_Correct_Value<int>();
        }

        [Property(Skip = "Decimal precision is not supported")]
        public Property CreateBasics_Decimal_Create_Correct_Objects()
        {
            return CreateBasics_Returns_Correct_Value<decimal>();
        }

        [Property]
        public Property CreateBasics_Double_Create_Correct_Objects()
        {
            return CreateBasics_Returns_Correct_Value<double>(number => !double.IsInfinity(number));
        }

        [Property]
        public Property CreateBasics_DateTime_Create_Correct_Objects()
        {
            return CreateBasics_Returns_Correct_Value<DateTime>();
        }

        [Property]
        public Property CreateBasics_String_Create_Correct_Objects()
        {
            Func<string, bool> when = Always<string>;
            if (!SupportStringEmpty)
                when = StringIsNotEmpty;

            return CreateBasics_Returns_Correct_Value(when);
        }

        private static bool Always<T>(T value) => true;

        private static bool StringIsNotEmpty(string value) => value != String.Empty;

        public Property CreateBasics_Returns_Correct_Value<T>(Func<T,bool> when = null)
        {
            when = when ?? Always;
            return Prop.ForAll<T>(value =>
                Test(() =>
                {
                    var res = ConvertBack(value);
                    var equal = EqualityComparer<T>.Default.Equals(res, value);
                    _Logger.Info($"{value} => {res} : {equal}");
                    return equal;
                }).When(when(value))
            );
        }

        [Property]
        public Property CreateBasics_Returns_Correct_Value_With_List()
        {
            return Prop.ForAll<object[]>(value =>
            {
                var converted = GetBack(value.ToList());
                return converted.SequenceEqual(value);
            });
        }

        private T ConvertBack<T>(T value)
        {
            if (value == null)
                return default(T);

            object res = null;
            Converter.GetSimpleValue(Get(value), out res, typeof(T));
            return (T)res;
        }

        private object ConvertBack(IJavascriptObject value, Type targetType)
        {
            object res = null;
            Converter.GetSimpleValue(value, out res, targetType);
            return res;
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
                yield return new object[] { new DateTime(1893, 11, 12, 14, 59, 11, DateTimeKind.Utc) };
            }
        }

        [Theory]
        [MemberData(nameof(DateTimes))]
        public void CreateBasics_DateTime_Create_Correct_Objects_Fixed_Values(DateTime date)
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
        public void CreateBasics_Uint_Create_Correct_Object()
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
