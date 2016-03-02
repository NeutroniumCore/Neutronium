using IntegratedTest.Windowless;
using MVVM.Awesomium.Tests.Infra;
using Xunit;

namespace MVVM.Awesomium.Tests.Integrated
{
    public class Test_HTMLBinding_Awesomium : Test_HTMLBinding
    {
        
        public Test_HTMLBinding_Awesomium() : base(AwesomiumTestHelper.GetWindowlessEnvironment())
        {
        }

        [Fact]
        public void Test()
        { }
    }
}

