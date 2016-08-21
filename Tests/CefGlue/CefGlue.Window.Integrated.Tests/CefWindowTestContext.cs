using CefGlue.TestInfra;
using Xunit;

namespace CefGlue.Window.Integrated.Tests
{
    [CollectionDefinition("Cef Window Integrated")]
    public class CefWindowTestContext : ICollectionFixture<CefWindowTestEnvironmentKo>
    {
    }
}
