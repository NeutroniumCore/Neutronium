using IntegratedTest.Tests.Windowless;
using MVVM.Awesomium.TestInfra;
using Xunit;
using Xunit.Abstractions;

namespace MVVM.Awesomium.Windowless.Integrated.Tests 
{
    [Collection("Awesomium Windowless Integrated")]
    public class Test_WebView_Dispatcher_Awesomium : Test_WebView_Dispatcher 
    {
        public Test_WebView_Dispatcher_Awesomium(AwesomiumTestContext context, ITestOutputHelper output)
            : base(context, output)
        {
        }
    }
}
