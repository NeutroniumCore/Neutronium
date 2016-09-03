using IntegratedTest.Tests.WPF;
using Ko.Navigation.CefGlue.Tests.Infra;
using Xunit;

namespace Ko.Navigation.CefGlue.Tests
{
    [Collection("Cef Window Integrated")]
    public class HTMLViewControl_Cef_Ko_Tests : HTMLViewControlTests
    {
        public HTMLViewControl_Cef_Ko_Tests(CefGlueKoContext context) : base(context)
        {
        }
    }
}
