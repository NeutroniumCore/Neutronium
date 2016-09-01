using Tests.Awesomium.Infra;
using Xunit;

namespace MVVM.Awesomium.Window.Integrated.Tests
{
    [CollectionDefinition("Awesomium Window Integrated")]
    public class AwesomiumWindowTestContext : ICollectionFixture<AwesomiumWindowTestEnvironment>
    {
    }
}
