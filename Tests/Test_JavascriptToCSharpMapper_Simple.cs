using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FluentAssertions;
using NSubstitute;
using Xunit;
using Xilium.CefGlue;

using MVVM.CEFGlue.HTMLBinding;


namespace MVVM.CEFGlue.Test
{
    public class Test_JavascriptToCSharpMapper_Simple : MVVMCefGlue_Test_Base
    {

        private JavascriptToCSharpMapper _JavascriptToCSharpMapper;
        public Test_JavascriptToCSharpMapper_Simple()
        {
            _JavascriptToCSharpMapper = new JavascriptToCSharpMapper();
        }
        
        [Fact]
        public void Test_GetSimpleValue_String()
        {
            Test(() =>
                {
                    object res = null;
                    bool ok = _JavascriptToCSharpMapper.GetSimpleValue(CefV8Value.CreateString("titi"), out res);
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
                   bool ok = _JavascriptToCSharpMapper.GetSimpleValue(CefV8Value.CreateInt(10), out res);
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
                   bool ok = _JavascriptToCSharpMapper.GetSimpleValue(CefV8Value.CreateBool(false), out res);
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
                   bool ok = _JavascriptToCSharpMapper.GetSimpleValue(CefV8Value.CreateBool(true), out res);
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
                   bool ok = _JavascriptToCSharpMapper.GetSimpleValue(CefV8Value.CreateDouble(0.5), out res);
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
                  bool ok = _JavascriptToCSharpMapper.GetSimpleValue(CefV8Value.CreateUndefined(), out res);
                  ok.Should().BeTrue();
                  res.Should().Be(null);
              });
        }

        [Fact]
        public void Test_GetSimpleValue_Null()
        {
            Test(() =>
            {
                object res = null;
                bool ok = _JavascriptToCSharpMapper.GetSimpleValue(CefV8Value.CreateNull(), out res);
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
                bool ok = _JavascriptToCSharpMapper.GetSimpleValue(CefV8Value.CreateDate(date), out res);
                ok.Should().BeTrue();
                res.Should().Be(date);
            });
        }

        [Fact]
        public void Test_GetSimpleValue_Object()
        {
            Test(() =>
            {
                object res = null;
                bool ok = _JavascriptToCSharpMapper.GetSimpleValue(CefV8Value.CreateObject(null), out res);
                ok.Should().BeFalse();
                res.Should().BeNull();
            });
        }

        //[Fact]
        //public void Test_GetSimpleValue_Uint()
        //{
        //    Test(() =>
        //    {
        //        object res = null;
        //        bool ok = _JavascriptToCSharpMapper.GetSimpleValue(CefV8Value.CreateUInt(uint.MaxValue), out res);
        //        ok.Should().BeTrue();
        //        res.Should().Be(uint.MaxValue);
        //    });
        //}

        [Fact]
        public void Test_GetSimpleValue_Uint_explicit()
        {
            Test(() =>
            {
                object res = null;
                bool ok = _JavascriptToCSharpMapper.GetSimpleValue(CefV8Value.CreateUInt(uint.MaxValue), out res, typeof(UInt32));
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
                bool ok = _JavascriptToCSharpMapper.GetSimpleValue(CefV8Value.CreateInt(-1), out res);
                ok.Should().BeTrue();
                res.Should().Be(-1);
            });
        }
    }
}
