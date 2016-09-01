using IntegratedTest.Tests.WPF;
using Tests.Awesomium.Infra;
using Xunit;

namespace MVVM.Awesomium.Window.Integrated.Tests 
{
    [Collection("Awesomium Window Integrated")]
    public class Test_DoubleNavigation_Animation_Awesomium : Test_DoubleNavigation_Animation
    {
        public Test_DoubleNavigation_Animation_Awesomium(AwesomiumWindowTestEnvironment context) : base(context) 
        {
        }
    }
}
