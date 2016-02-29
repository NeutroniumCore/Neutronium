using System;
using NSubstitute;
using Xunit;
using FluentAssertions;
using MVVM.Cef.Glue.Test.Core;
using IntegratedTest.Windowless;
using MVVM.Cef.Glue.Test.Infra;

namespace MVVM.Cef.Glue.Test
{
    public class Test_CefTask_Action_Cef : Test_CefTask_Action
    {
        public Test_CefTask_Action_Cef(): base(CefTestHelper.GetWindowlessEnvironment())
        {
        }
    }
}
