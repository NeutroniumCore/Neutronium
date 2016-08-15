using Xunit;

namespace ChromiumFX.Windowless.Integrated.Tests 
{

    [CollectionDefinition("ChromiumFX Vue Windowless Integrated")]
    public class ChromiumFXWindowlessTestContext : ICollectionFixture<ChromiumFXWindowLessHTMLEngineProviderVue> 
    {
    }
}
