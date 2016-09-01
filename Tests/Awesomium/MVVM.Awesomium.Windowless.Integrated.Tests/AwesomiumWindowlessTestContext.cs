using Tests.Awesomium.Infra;
using Xunit;

namespace MVVM.Awesomium.Windowless.Integrated.Tests {

    [CollectionDefinition("Awesomium Windowless Integrated")]
    public class AwesomiumWindowlessTestContext : ICollectionFixture<AwesomiumTestContext> 
    {
    }
}
