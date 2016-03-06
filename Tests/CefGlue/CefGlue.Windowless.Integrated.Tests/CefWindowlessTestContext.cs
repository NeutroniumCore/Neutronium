using CefGlue.TestInfra;
using Xunit;

namespace CefGlue.Windowless.Integrated.Tests {

    [CollectionDefinition("Cef Windowless Integrated")]
    public class CefWindowlessTestContext : ICollectionFixture<CefGlueWindowlessSharedJavascriptEngineFactory> 
    {
    }
}
