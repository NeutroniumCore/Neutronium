using Xunit;

namespace Tests.CefGlue.WebBrowserEngineTests.Infra
{
    [CollectionDefinition("CefGlue Context")]
    public class CefGlueContextFixture : ICollectionFixture<CefGlueContext> 
    {
    }
}