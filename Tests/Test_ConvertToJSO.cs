using System;
using System.Collections.Generic;
using System.Collections;

using FluentAssertions;
using NSubstitute;
using Xunit;

using MVVM.HTML.Core.HTMLBinding;
using MVVM.Cef.Glue.Test.CefWindowless;
using MVVM.Cef.Glue.Test.Core;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.Binding.Mapping;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.Cef.Glue.Test
{
    public class Test_ConvertToJSO : MVVMCefGlue_Test_Base
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


        private CSharpToJavascriptConverter _ConverTOJSO;
        //private CefV8_Factory _IJSOBuilder;
        private TestClass _Test;
        private Test2 _Test2;
        private List<TestClass> _Tests;
        private ArrayList _Tests_NG;
        private HTMLViewContext _HTMLViewContext;

        private IJavascriptSessionCache _ICSharpMapper;

        public Test_ConvertToJSO()
        {
        }

        protected override void Init()
        {
            _ICSharpMapper = Substitute.For<IJavascriptSessionCache>();
            _ICSharpMapper.GetCached(Arg.Any<object>()).Returns((IJSCSGlue)null);
            _HTMLViewContext = new HTMLViewContext(_WebView, new TestIUIDispatcher(), new KnockoutSessionInjectorFactory());
            _ConverTOJSO = new CSharpToJavascriptConverter(_HTMLViewContext, _ICSharpMapper, new CommandFactory(null));
            _Test = new TestClass { S1 = "string", I1 = 25 };
            _Tests = new List<TestClass>();
            _Tests.Add(new TestClass() { S1 = "string1", I1 = 1 });
            _Tests.Add(new TestClass() { S1 = "string2", I1 = 2 });
            _Test2 = new Test2() { T1 = _Test, T2 = _Test };

            _Tests_NG = new ArrayList();
            _Tests_NG.Add(_Tests[0]);
            _Tests_NG.Add(_Tests[0]);
        }

        [Fact]
        public void Test_Simple()
        {
            Test(() =>
                {
                    var res = _ConverTOJSO.Map(_Test).JSValue;
                    res.Should().NotBeNull();
                    var res1 = res.GetValue("S1");
                    res1.Should().NotBeNull();
                    res1.Convert().IsString.Should().BeTrue();

                    var res2 = res.GetValue("I1");
                    res2.Should().NotBeNull();
                    res2.Convert().IsInt.Should().BeTrue();
                });
        }

        [Fact]
        public void Test_List()
        {
            Test(() =>
              {
                  var ibridgeresult = _ConverTOJSO.Map(_Tests);
                  ibridgeresult.Type.Should().Be(JSCSGlueType.Array);
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
        }

        [Fact]
        public void Test_List_Not_Generic()
        {
            Test(() =>
              {
                  var resv = _ConverTOJSO.Map(_Tests_NG).JSValue;

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
        }

        [Fact]
        public void Test_Double()
        {
            Test(() =>
              {
                  var res = _ConverTOJSO.Map(0.2D).JSValue;
                  res.Should().NotBeNull();
                  res.IsNumber.Should().BeTrue();
                  double resd = res.GetDoubleValue();

                  resd.Should().Be(0.2D);
              });
        }

         [Fact]
        public void Test_Date()
        {
            Test(() =>
              {
                  var date = new DateTime(1974, 02, 26, 01, 02, 03,DateTimeKind.Utc);
                  var res = _ConverTOJSO.Map(date).JSValue;
                  res.Should().NotBeNull();
                  res.IsDate.Should().BeTrue();
                  var convert = new CefV8_Converter();
                  object ores = null;

                  convert.GetSimpleValue(res, out ores);
                  var resd = (DateTime)ores;

                  resd.Should().Be(date);
              });
        }

        [Fact]
        public void Test_Decimal()
        {
            Test(() =>
              {
                  var res = _ConverTOJSO.Map(0.2M).JSValue;
                  res.Should().NotBeNull();
                  res.IsNumber.Should().BeTrue();
                  double resd = res.GetDoubleValue();

                  resd.Should().Be(0.2D);
              });
        }


        [Fact]
        public void Test_Bool()
        {
            Test(() =>
              {
                  var res = _ConverTOJSO.Map(true).JSValue;
                  res.Should().NotBeNull();
                  res.IsBool.Should().BeTrue();
                  bool resd = res.GetBoolValue();

                  resd.Should().BeTrue();
              });
        }

        [Fact]
        public void Test_Bool_False()
        {
            Test(() =>
                {
                    var res = _ConverTOJSO.Map(false).JSValue;
                    res.Should().NotBeNull();
                    res.IsBool.Should().BeTrue();
                    bool resd = res.GetBoolValue();

                    resd.Should().BeFalse();
                });
        }

        [Fact]
        public void Test_String()
        {
            Test(() =>
              {
                  var res = _ConverTOJSO.Map("toto").JSValue;
                  res.Should().NotBeNull();
                  res.IsString.Should().BeTrue();
                  string resd = res.GetStringValue();

                  resd.Should().Be("toto");
              });
        }

        [Fact]
        public void Test_Object_Double_reference()
        {
            Test(() =>
              {
                  var res = _ConverTOJSO.Map(_Test2).JSValue;
                  res.Should().NotBeNull();

                  _ICSharpMapper.Received().Cache(_Test, Arg.Any<IJSCSGlue>());
              });
        }
    }
}
