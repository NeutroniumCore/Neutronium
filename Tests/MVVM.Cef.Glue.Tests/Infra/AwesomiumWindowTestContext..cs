using CefGlue.TestInfra;
using Xunit;

namespace MVVM.Cef.Glue.Tests.Infra
{
    [CollectionDefinition("Cef Window Integrated")]
    public class CefWindowTestContext : ICollectionFixture<CefWindowTestEnvironment>
    {
    }
}
