using MVVM.Awesomium.TestInfra;
using Xunit;

namespace MVVM.Awesomium.Windowless.Integrated.Tests {

    [CollectionDefinition("Awesomium Windowless Integrated")]
    public class AwesomiumWindowlessTestContext : ICollectionFixture<AwesomiumTestContext> 
    {
    }
}
