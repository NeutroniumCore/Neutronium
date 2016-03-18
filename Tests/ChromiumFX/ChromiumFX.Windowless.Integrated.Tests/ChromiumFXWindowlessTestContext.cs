using ChromiumFX.TestInfra;
using Xunit;

namespace ChromiumFX.Windowless.Integrated.Tests 
{

    [CollectionDefinition("ChromiumFX Windowless Integrated")]
    public class ChromiumFXWindowlessTestContext : ICollectionFixture<ChromiumFXWindowLessHTMLEngineProvider> 
    {
    }
}
