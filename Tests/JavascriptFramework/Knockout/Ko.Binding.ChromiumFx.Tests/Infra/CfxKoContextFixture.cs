using Xunit;

namespace Ko.Binding.ChromiumFx.Tests.Infra
{
    [CollectionDefinition("Cfx Ko Windowless Integrated")]
    public class CefGlueKoContextFixture : ICollectionFixture<CfxKoContext>
    {
    }
}
