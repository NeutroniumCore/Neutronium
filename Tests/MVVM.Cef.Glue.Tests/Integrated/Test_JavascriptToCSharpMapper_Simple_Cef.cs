using System;
using FluentAssertions;
using Xunit;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using IntegratedTest.Windowless;
using MVVM.Cef.Glue.Test.Infra;

namespace MVVM.Cef.Glue.Test
{
    public class Test_JavascriptToCSharpMapper_Simple_Cef : Test_JavascriptToCSharpMapper_Simple
    {
        public Test_JavascriptToCSharpMapper_Simple_Cef(): base(CefTestHelper.GetWindowlessEnvironment())
        {
        }
    }
}
