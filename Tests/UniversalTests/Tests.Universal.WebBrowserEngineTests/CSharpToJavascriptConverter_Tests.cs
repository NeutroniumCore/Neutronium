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
        private ArrayList _Tests_NG;
        private HTMLViewContext _HTMLViewContext;
        private IGlueFactory _GlueFactory;
        private IJavascriptSessionCache _ICSharpMapper;
        private IJavascriptFrameworkManager _javascriptFrameworkManager;
        private IWebBrowserWindow WebBrowserWindow => _WebBrowserWindowProvider.HTMLWindow;

        protected CSharpToJavascriptConverter_Tests(IBasicWindowLessHTMLEngineProvider testEnvironment, ITestOutputHelper output)
            : base(testEnvironment, output)
        {
        }

        protected override void Init()
        {
            _ICSharpMapper = Substitute.For<IJavascriptSessionCache>();
            _GlueFactory = new GlueFactory(null, null);
            _ICSharpMapper.GetCached(Arg.Any<object>()).Returns((IJSCSGlue)null);
            _javascriptFrameworkManager = Substitute.For<IJavascriptFrameworkManager>();
            _HTMLViewContext = new HTMLViewContext(WebBrowserWindow, GetTestUIDispacther(), _javascriptFrameworkManager, null, _Logger);
            _ConverTOJSO = new CSharpToJavascriptConverter(WebBrowserWindow, _ICSharpMapper, _GlueFactory, _Logger);
            _Test = new TestClass { S1 = "string", I1 = 25 };
            _Tests = new List<TestClass>
            {
                new TestClass() {  S1 = "string1", I1 = 1 },
                new TestClass() { S1 = "string2", I1 = 2  }
            };
            _Test2 = new Test2() { T1 = _Test, T2 = _Test };

            _Tests_NG = new ArrayList();
            _Tests_NG.Add(_Tests[0]);
            _Tests_NG.Add(_Tests[0]);

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
                var res = (await Map(_Test)).JSValue;

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
        public async Task Test_Circular_Reference_Simple() {    
            await TestAsync(async () => 
            {
                var cacher = new SessionCacher();
                _ConverTOJSO = GetCircularBreakerConverter(cacher);

                var res = (await Map(_CircularSimple, cacher)).JSValue;

                DoSafe(() => {
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

                var res = (await Map(_CircularComplex, cacher)).JSValue;

                DoSafe(() => {
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
            return new CSharpToJavascriptConverter(WebBrowserWindow, cacher, _GlueFactory, _Logger);
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
                    IJavascriptObject resv = ibridgeresult.JSValue;

                    resv.Should().NotBeNull();
                    resv.IsArray.Should().BeTrue();
                    resv.GetArrayLength().Should().Be(2);

                    IJavascriptObject res = resv.GetValue(0);
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
                    int v2 = res2.GetIntValue();
                    v2.Should().Be(1);
                });
            });
        }

        [Fact]
        public async Task Test_List_Not_Generic()
        {
            await TestAsync(async () =>
            {
                var resv = (await Map(_Tests_NG)).JSValue;

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
                    int v2 = res2.GetIntValue();
                    v2.Should().Be(1);
                });
            });
        }

        [Fact]
        public async Task Test_Double()
        {
            await TestAsync(async () =>
            {
                var res = (await Map(0.2D)).JSValue;
                res.Should().NotBeNull();
                res.IsNumber.Should().BeTrue();
                double resd = res.GetDoubleValue();

                resd.Should().Be(0.2D);
            });
        }

        [Fact]
        public async Task Test_Date()
        {
            await TestAsync(async () =>
            {
                var date = new DateTime(1974, 02, 26, 01, 02, 03, DateTimeKind.Utc);
                var res = (await Map(date)).JSValue;
                res.Should().NotBeNull();

                object ores = null;
                Converter.GetSimpleValue(res, out ores);
                var resd = (DateTime)ores;

                resd.Should().Be(date);
            });
        }

        [Fact]
        public async Task Test_Decimal()
        {
            await TestAsync(async () =>
            {
                var res = (await Map(0.2M)).JSValue;
                res.Should().NotBeNull();
                res.IsNumber.Should().BeTrue();
                double resd = res.GetDoubleValue();

                resd.Should().Be(0.2D);
            });
        }


        [Fact]
        public async Task Test_Bool()
        {
            await TestAsync(async () =>
            {
                var res = (await Map(true)).JSValue;
                res.Should().NotBeNull();
                res.IsBool.Should().BeTrue();
                bool resd = res.GetBoolValue();

                resd.Should().BeTrue();
            });
        }

        [Fact]
        public async Task Test_Bool_False()
        {
            await TestAsync(async () =>
                {
                    var res = (await Map(false)).JSValue;
                    res.Should().NotBeNull();
                    res.IsBool.Should().BeTrue();
                    bool resd = res.GetBoolValue();

                    resd.Should().BeFalse();
                });
        }

        [Fact]
        public async Task Test_String()
        {
            await TestAsync(async () =>
              {
                  var res = (await Map("toto")).JSValue;
                  res.Should().NotBeNull();
                  res.IsString.Should().BeTrue();
                  string resd = res.GetStringValue();

                  resd.Should().Be("toto");
              });
        }

        [Fact]
        public async Task Test_Object_Double_reference()
        {
            await TestAsync(async () =>
            {
                var res = (await Map(_Test2)).JSValue;
                res.Should().NotBeNull();

                _ICSharpMapper.Received().Cache(_Test, Arg.Any<IJSCSGlue>());
            });
        }

        private async Task<IJSCSGlue> Map(object from, IJavascriptSessionCache cacher=null)
        {
            cacher = cacher ?? _ICSharpMapper;
            var res = await _HTMLViewContext.EvaluateOnUIContextAsync(() => _ConverTOJSO.Map(from));
            await _HTMLViewContext.RunOnJavascriptContextAsync(() =>
            {
                var builder = _HTMLViewContext.WebView.GetBuildingStrategy(cacher);
                builder.UpdateJavascriptValue(res);
            });
            return res;
        }
    }
}
