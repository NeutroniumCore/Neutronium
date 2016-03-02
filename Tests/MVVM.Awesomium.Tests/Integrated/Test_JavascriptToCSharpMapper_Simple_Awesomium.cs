using IntegratedTest.Windowless;
using MVVM.Awesomium.Tests.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.Awesomium.Tests.Integrated
{
    public class Test_JavascriptToCSharpMapper_Simple_Awesomium : Test_JavascriptToCSharpMapper_Simple
    {
        public Test_JavascriptToCSharpMapper_Simple_Awesomium() 
            : base(AwesomiumTestHelper.GetWindowlessEnvironment())
        {
        }
    }
}
