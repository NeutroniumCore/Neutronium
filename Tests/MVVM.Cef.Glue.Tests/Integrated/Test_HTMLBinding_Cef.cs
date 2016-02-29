using IntegratedTest.Windowless;
using MVVM.Cef.Glue.Tests.Infra;

namespace MVVM.Cef.Glue.Tests.Integrated
{
    public class Test_HTMLBinding_Cef : Test_HTMLBinding
    {
        public Test_HTMLBinding_Cef(): base(CefTestHelper.GetWindowlessEnvironment())
        {
        }
    }
}

