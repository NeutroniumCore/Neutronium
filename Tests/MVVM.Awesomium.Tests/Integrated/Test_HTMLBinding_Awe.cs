using IntegratedTest.Windowless;
using MVVM.Awesomium.Tests.Infra;
using Xunit;

namespace MVVM.Awesomium.Tests.Integrated
{
    public class Test_HTMLBinding_Awe : Test_HTMLBinding
    {
        
        public Test_HTMLBinding_Awe() : base(AwesomiumTestHelper.GetWindowlessEnvironment())
        {
        }

        [Fact]
        public void Test()
        { }
    }
}

