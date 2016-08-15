using ChromiumFX.TestInfra;
using Xunit;

namespace ChromiumFX.Windowless.Integrated.Tests 
{

    [CollectionDefinition("ChromiumFX Ko Windowless Integrated")]
    public class ChromiumFXWindowlessTestContext : ICollectionFixture<ChromiumFXWindowLessHTMLEngineProviderKo> 
    {
    }
}
