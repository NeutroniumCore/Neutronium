using IntegratedTest.Tests.Windowless;
using MVVM.Awesomium.TestInfra;
using Xunit;

namespace MVVM.Awesomium.Windowless.Integrated.Tests 
{
    [Collection("Awesomium Windowless Integrated")]
    public class Test_WebView_Dispatcher_Awesomium : Test_WebView_Dispatcher 
    {
        public Test_WebView_Dispatcher_Awesomium(AwesomiumTestContext context) : base(context)
        {
        }
    }
}
