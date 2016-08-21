using Xunit;

namespace CefGlue.Windowless.Integrated.Tests
{

    [CollectionDefinition("Cef Windowless Ko Integrated")]
    public class CefWindowlessTestContext : ICollectionFixture<CefGlueWindowlessSharedJavascriptEngineFactoryKo> 
    {
    }
}
