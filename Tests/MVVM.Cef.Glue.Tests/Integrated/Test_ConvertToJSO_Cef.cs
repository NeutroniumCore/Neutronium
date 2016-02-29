using System;
using System.Collections.Generic;
using System.Collections;

using FluentAssertions;
using NSubstitute;
using Xunit;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.Binding.GlueObject;
using MVVM.HTML.Core.JavascriptUIFramework;
using IntegratedTest.Windowless;
using MVVM.Cef.Glue.Test.Infra;

namespace MVVM.Cef.Glue.Test
{
    public class Test_ConvertToJSO_Cef : Test_ConvertToJSO
    {
        public Test_ConvertToJSO_Cef() : base(CefTestHelper.GetWindowlessEnvironment())
        {
        }
    }
}
