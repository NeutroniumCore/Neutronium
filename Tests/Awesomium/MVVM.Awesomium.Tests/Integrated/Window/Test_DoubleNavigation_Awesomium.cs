using IntegratedTest.WPF;
using MVVM.Awesomium.TestInfra;
using Xunit;

namespace MVVM.Awesomium.Tests.Integrated.Window 
{
    [Collection("Awesomium Window Integrated")]
    public class Test_DoubleNavigation_Awesomium : Test_DoubleNavigation
    {
        public Test_DoubleNavigation_Awesomium(AwesomiumWindowTestEnvironment context) : base(context) 
        {         
        }
    }
}
