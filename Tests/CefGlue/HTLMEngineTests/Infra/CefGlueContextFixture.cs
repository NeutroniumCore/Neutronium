using Xunit;

namespace Tests.CefGlue.HTMLEngineTests.Infra
{
    [CollectionDefinition("CefGlue Context")]
    public class CefGlueContextFixture : ICollectionFixture<CefGlueContext> 
    {
    }
}