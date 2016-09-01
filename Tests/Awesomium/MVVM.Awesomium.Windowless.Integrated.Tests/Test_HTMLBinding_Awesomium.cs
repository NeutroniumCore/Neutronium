using IntegratedTest.Tests.Windowless;
using Tests.Awesomium.Infra;
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

