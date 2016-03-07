using IntegratedTest.WPF;
using IntegratedTest.WPF.Infra;
using MVVM.Awesomium.TestInfra;
using Xunit;

namespace MVVM.Awesomium.Tests.Integrated.Window 
{
    public class Test_DoubleNavigation_Awesomium : Test_DoubleNavigation, IClassFixture<AwesomiumWindowTestEnvironment>
    {
        public Test_DoubleNavigation_Awesomium(AwesomiumWindowTestEnvironment context, WpfThread wpfThread) : base(context, wpfThread) 
        {
            
        }
    }
}
