using Xunit;

namespace CefGlue.Windowless.Integrated.Ko.Tests
{

    [CollectionDefinition("Cef Windowless Ko Integrated")]
    public class CefWindowlessTestContext : ICollectionFixture<CefGlueWindowlessSharedJavascriptEngineFactoryKo> 
    {
    }
}
