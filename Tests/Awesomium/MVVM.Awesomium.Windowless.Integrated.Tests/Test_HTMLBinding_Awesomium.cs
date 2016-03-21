using IntegratedTest.Tests.Windowless;
using MVVM.Awesomium.TestInfra;
using Xunit;
using Xunit.Abstractions;

namespace MVVM.Awesomium.Windowless.Integrated.Tests
{
    [Collection("Awesomium Windowless Integrated")]
    public class Test_HTMLBinding_Awesomium : Test_HTMLBinding
    {

        public Test_HTMLBinding_Awesomium(AwesomiumTestContext context, ITestOutputHelper output)
            : base(context, output)
        {
        }
    }
}

