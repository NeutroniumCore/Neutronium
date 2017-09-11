using Ko.Navigation.CefGlue.Tests.Infra;
using Tests.Universal.NavigationTests;
using Xunit;
using Xunit.Abstractions;

namespace Ko.Navigation.CefGlue.Tests
{
    [Collection("Cef Window Integrated")]
    public class HTMLViewControl_Ko_Cef_Tests : HTMLViewControlTests
    {
        public HTMLViewControl_Ko_Cef_Tests(CefGlueKoContext context, ITestOutputHelper testOutputHelper) 
            : base(context, testOutputHelper)
        {
        }
    }
}
