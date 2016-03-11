using IntegratedTest.Tests.Windowless;
using MVVM.Awesomium.TestInfra;
using Xunit;

namespace MVVM.Awesomium.Windowless.Integrated.Tests
{
    [Collection("Awesomium Windowless Integrated")]
    public class Test_HTMLBinding_Awesomium : Test_HTMLBinding
    {
        
        public Test_HTMLBinding_Awesomium(AwesomiumTestContext context) : base(context)
        {
        }
    }
}

