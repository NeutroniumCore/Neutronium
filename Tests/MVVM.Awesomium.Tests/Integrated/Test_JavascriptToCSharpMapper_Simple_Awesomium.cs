using IntegratedTest.Windowless;
using MVVM.Awesomium.Tests.Infra;

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
