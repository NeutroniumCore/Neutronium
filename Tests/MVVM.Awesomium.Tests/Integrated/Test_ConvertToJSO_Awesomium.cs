using IntegratedTest.Windowless;
using MVVM.Awesomium.Tests.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.Awesomium.Tests.Integrated
{
    public class Test_ConvertToJSO_Awesomium  : Test_ConvertToJSO
    {
        public Test_ConvertToJSO_Awesomium(): base(AwesomiumTestHelper.GetWindowlessEnvironment())
        {
        }
    }
}
