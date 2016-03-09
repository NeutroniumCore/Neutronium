using MVVM.Awesomium.TestInfra;
using Xunit;

namespace MVVM.Awesomium.Window.Integrated.Tests
{
    [CollectionDefinition("Awesomium Window Integrated")]
    public class AwesomiumWindowTestContext : ICollectionFixture<AwesomiumWindowTestEnvironment>
    {
    }
}
