using IntegratedTest.Windowless;
using MVVM.Awesomium.TestInfra;
using Xunit;

namespace MVVM.Awesomium.Windowless.Integrated.Tests
{
    [Collection("Awesomium Windowless Integrated")]
    public class Test_HTMLBinding_Awesomium : Test_HTMLBinding, IClassFixture<AwesomiumTestContext> {
        
        public Test_HTMLBinding_Awesomium(AwesomiumTestContext context) : base(context.GetWindowlessEnvironment())
        {
        }
    }
}

