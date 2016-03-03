using IntegratedTest.Windowless;
using MVVM.Awesomium.Tests.Infra;
using Xunit;

namespace MVVM.Awesomium.Tests.Integrated
{
    public class Test_HTMLBinding_Awesomium : Test_HTMLBinding, IClassFixture<AwesomiumTestContext> {
        
        public Test_HTMLBinding_Awesomium(AwesomiumTestContext context) : base(context.GetWindowlessEnvironment())
        {
        }

        [Fact]
        public void Test()
        { }
    }
}

