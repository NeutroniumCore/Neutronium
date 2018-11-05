using System;
using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using Neutronium.Core.Binding;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using NSubstitute;
using Tests.Infra.WebBrowserEngineTesterHelper.Context;
using Tests.Infra.WebBrowserEngineTesterHelper.Windowless;
using Xunit;
using Xunit.Abstractions;
using System.Threading.Tasks;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.GlueBuilder;
using Neutronium.Core.Binding.Listeners;

namespace Tests.Universal.WebBrowserEngineTests
{
    public abstract class CSharpToJavascriptConverter_Tests : TestBase
    {
        private class TestClass
        {
            public string S1 { get; set; }
            public int I1 { get; set; }
        }

        private class Test2
        {
            public TestClass T1 { get; set; }
            public TestClass T2 { get; set; }
        }

        private class Circular1
        {
            public Circular1 Reference { get; set; }
        }

        private class Circular2
        {
            public Circular2 Reference { get; set; }
            public List<Circular2> List { get; } = new List<Circular2>();
        }

        private CSharpToJavascriptConverter _ConverTOJSO;
        private TestClass _Test;
        private Test2 _Test2;
        private Circular1 _CircularSimple;
        private Circular2 _CircularComplex;
        private List<TestClass> _Tests;
        private ArrayList _TestsNg;
        private HtmlViewContext _HtmlViewContext;
        private IGlueFactory _GlueFactory;
        private IJavascriptSessionCache _CSharpMapper;
        private IJavascriptFrameworkManager _JavascriptFrameworkManager;
        private IWebBrowserWindow WebBrowserWindow => _WebBrowserWindowProvider.HtmlWindow;
        private ObjectChangesListener _ObjectChangesListener;


        protected CSharpToJavascriptConverter_Tests(IBasicWindowLessHTMLEngineProvider testEnvironment, ITestOutputHelper output)
            : base(testEnvironment, output)
        {
        }

        protected override void Init()
        {
            _CSharpMapper = Substitute.For<IJavascriptSessionCache>();
            _ObjectChangesListener = new ObjectChangesListener(_ => { }, _ => { }, _ => { });
            _GlueFactory = new GlueFactory(null, _CSharpMapper, null, _ObjectChangesListener);
            _CSharpMapper.GetCached(Arg.Any<object>()).Returns((IJsCsGlue)null);
            _JavascriptFrameworkManager = Substitute.For<IJavascriptFrameworkManager>();
            _HtmlViewContext = new HtmlViewContext(WebBrowserWindow, GetTestUIDispacther(), _JavascriptFrameworkManager, null, _Logger);
            _ConverTOJSO = new CSharpToJavascriptConverter(_CSharpMapper, _GlueFactory, _Logger);
            _Test = new TestClass { S1 = "string", I1 = 25 };
            _Tests = new List<TestClass>
            {
                new TestClass() {  S1 = "string1", I1 = 1 },
                new TestClass() { S1 = "string2", I1 = 2  }
            };
            _Test2 = new Test2() { T1 = _Test, T2 = _Test };

            _TestsNg = new ArrayList 
            {
                _Tests[0],
                _Tests[0]
            };

            _CircularSimple = new Circular1();
            _CircularSimple.Reference = _CircularSimple;

            _CircularComplex = new Circular2();
            var circularChild = new Circular2
            {
                Reference = _CircularComplex
            };
            _CircularComplex.List.Add(circularChild);
        }

        [Fact]
        public async Task Test_Simple()
        {
            await TestAsync(async () =>
            {
                var res = (await Map(_Test)).JsValue;

                DoSafe(() =>
                {
                    res.Should().NotBeNull();
                    var res1 = res.GetValue("S1");
                    res1.Should().NotBeNull();
                    res1.IsString.Should().BeTrue();
                    var res2 = res.GetValue("I1");
                    res2.Should().NotBeNull();
                    res2.IsNumber.Should().BeTrue();
                });
            });
        }

        [Fact]
        public async Task Test_Circular_Reference_Simple()
        {
            await TestAsync(async () =>
            {
                var cacher = new SessionCacher();
                _ConverTOJSO = GetCircularBreakerConverter(cacher);

                var res = (await Map(_CircularSimple, cacher)).JsValue;

                DoSafe(() =>
                {
                    res.Should().NotBeNull();
                    var res1 = res.GetValue("Reference");
                    res1.Should().NotBeNull();
                    res1.IsObject.Should().BeTrue();

                    var res2 = res1.GetValue("Reference");
                    res2.Should().NotBeNull();
                    res2.IsObject.Should().BeTrue();

                    var res3 = res1.GetValue("Reference");
                    res3.Should().NotBeNull();
                    res3.IsObject.Should().BeTrue();
                });
            });
        }

        [Fact]
        public async Task Test_Circular_Reference_In_List()
        {
            await TestAsync(async () =>
            {
                var cacher = new SessionCacher();
                _ConverTOJSO = GetCircularBreakerConverter(cacher);

                var res = (await Map(_CircularComplex, cacher)).JsValue;

                DoSafe(() =>
                {
                    res.Should().NotBeNull();

                    var res0 = res.GetValue("List");
                    res0.IsArray.Should().BeTrue();

                    var res1 = res0.GetArrayElements()[0];
                    res1.Should().NotBeNull();
                    res1.IsObject.Should().BeTrue();

                    var res2 = res1.GetValue("Reference");
                    res2.Should().NotBeNull();
                    res2.IsObject.Should().BeTrue();
                });
            });
        }

        private CSharpToJavascriptConverter GetCircularBreakerConverter(IJavascriptSessionCache cacher)
        {
            _GlueFactory = new GlueFactory(null, cacher, null, _ObjectChangesListener);
            return new CSharpToJavascriptConverter( cacher, _GlueFactory, _Logger);
        }

        [Fact]
        public async Task Test_List()
        {
            await TestAsync(async () =>
            {
                var ibridgeresult = await Map(_Tests);

                DoSafe(() =>
                {
                    ibridgeresult.Type.Should().Be(JsCsGlueType.Array);
                    var resv = ibridgeresult.JsValue;

                    resv.Should().NotBeNull();
                    resv.IsArray.Should().BeTrue();
                    resv.GetArrayLength().Should().Be(2);

                    var res = resv.GetValue(0);
                    res.Should().NotBeNull();
                    var res1 = res.GetValue("S1");
                    res1.Should().NotBeNull();
                    res1.IsString.Should().BeTrue();

                    var jsv = res.GetValue("S1");
                    jsv.Should().NotBeNull();
                    jsv.IsString.Should().BeTrue();
                    var stv = jsv.GetStringValue();
                    stv.Should().NotBeNull();
                    stv.Should().Be("string1");

                    var res2 = res.GetValue("I1");
                    res2.Should().NotBeNull();
                    res2.IsNumber.Should().BeTrue();
                    var v2 = res2.GetIntValue();
                    v2.Should().Be(1);
                });
            });
        }

        [Fact]
        public async Task Test_List_Not_Generic()
        {
            await TestAsync(async () =>
            {
                var resv = (await Map(_TestsNg)).JsValue;

                DoSafe(() =>
                {

                    resv.Should().NotBeNull();
                    resv.IsArray.Should().BeTrue();
                    resv.GetArrayLength().Should().Be(2);

                    var res = resv.GetValue(0);
                    res.Should().NotBeNull();
                    var res1 = res.GetValue("S1");
                    res1.Should().NotBeNull();
                    res1.IsString.Should().BeTrue();

                    var jsv = res.GetValue("S1");
                    jsv.Should().NotBeNull();
                    jsv.IsString.Should().BeTrue();
                    string stv = jsv.GetStringValue();
                    stv.Should().NotBeNull();
                    stv.Should().Be("string1");

                    var res2 = res.GetValue("I1");
                    res2.Should().NotBeNull();
                    res2.IsNumber.Should().BeTrue();
                    var v2 = res2.GetIntValue();
                    v2.Should().Be(1);
                });
            });
        }

        [Fact]
        public async Task Test_Double()
        {
            await TestAsync(async () =>
            {
                var res = (await Map(0.2D)).JsValue;

                DoSafe(() => 
                {
                    res.Should().NotBeNull();
                    res.IsNumber.Should().BeTrue();
                    var resd = res.GetDoubleValue();

                    resd.Should().Be(0.2D);
                });
            });
        }

        [Fact]
        public async Task Test_Date()
        {
            await TestAsync(async () =>
            {
                var date = new DateTime(1974, 02, 26, 01, 02, 03, DateTimeKind.Utc);
                var res = (await Map(date)).JsValue;

                DoSafe(() => 
                {
                    res.Should().NotBeNull();

                    Converter.GetSimpleValue(res, out var ores);
                    var resd = (DateTime) ores;

                    resd.Should().Be(date);
                });
            });
        }

        [Fact]
        public async Task Test_Decimal()
        {
            await TestAsync(async () =>
            {
                var res = (await Map(0.2M)).JsValue;

                DoSafe(() => 
                {
                    res.Should().NotBeNull();
                    res.IsNumber.Should().BeTrue();
                    double resd = res.GetDoubleValue();

                    resd.Should().Be(0.2D);
                });
            });
        }


        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Test_Bool(bool value)
        {
            await TestAsync(async () =>
            {
                var res = (await Map(value)).JsValue;
                DoSafe(() => 
                {
                    res.Should().NotBeNull();
                    res.IsBool.Should().BeTrue();
                    bool resd = res.GetBoolValue();

                    resd.Should().Be(value);
                });
            });
        }

        [Fact]
        public async Task Test_String()
        {
            await TestAsync(async () =>
              {
                  var res = (await Map("toto")).JsValue;

                  DoSafe(() => 
                  {
                      res.Should().NotBeNull();
                      res.IsString.Should().BeTrue();
                      string resd = res.GetStringValue();

                      resd.Should().Be("toto");
                  });
              });
        }

        [Fact]
        public async Task Test_Object_Double_reference()
        {
            await TestAsync(async () =>
            {
                var res = (await Map(_Test2)).JsValue;
                res.Should().NotBeNull();

                _CSharpMapper.Received().CacheFromCSharpValue(_Test, Arg.Any<IJsCsGlue>());
            });
        }

        private async Task<IJsCsGlue> Map(object from, IJavascriptSessionCache cacher = null)
        {
            cacher = cacher ?? _CSharpMapper;
            var res = await _HtmlViewContext.EvaluateOnUiContextAsync(() => _ConverTOJSO.Map(from));
            await _HtmlViewContext.RunOnJavascriptContextAsync(() =>
            {
                var builder = GetStrategy(_HtmlViewContext.WebView, cacher, false);
                builder.UpdateJavascriptValue(res);
            });
            return res;
        }


        private static IJavascriptObjectBuilderStrategy GetStrategy(IWebView webView, IJavascriptSessionCache cache, bool mapping)
        {
            return webView.AllowBulkCreation ? (IJavascriptObjectBuilderStrategy)new JavascriptObjectBulkBuilderStrategy(webView, cache, mapping) :
                                             new JavascriptObjectSynchroneousBuilderStrategy(webView, cache, mapping);
        }
    }
}
