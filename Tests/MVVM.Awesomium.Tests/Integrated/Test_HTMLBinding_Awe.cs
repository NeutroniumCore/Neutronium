using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using MVVM.HTML.Core;
using MVVM.HTML.Core.Exceptions;
using MVVM.HTML.Core.Binding.GlueObject;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

using IntegratedTest;
using IntegratedTest.TestData;
using IntegratedTest.Windowless;
using MVVM.Cef.Glue.Test.Infra;
using Xunit;

namespace MVVM.Awesomium.Tests.Integrated
{
    //[Fact]
    public class Test_HTMLBinding_Awe : Test_HTMLBinding
    {
        
        public Test_HTMLBinding_Awe() : base(AwesomiumTestHelper.GetWindowlessEnvironment())
        {
        }

        [Fact]
        public void Test()
        { }
    }
}

