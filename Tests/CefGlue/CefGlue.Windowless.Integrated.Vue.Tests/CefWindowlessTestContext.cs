using Xunit;

namespace CefGlue.Windowless.Integrated.Vue.Tests
{
    [CollectionDefinition("Cef Windowless Vue Integrated")]
    public class CefWindowlessTestContext : ICollectionFixture<CefGlueWindowlessSharedJavascriptEngineFactoryVue> 
    {
    }
}
