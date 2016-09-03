using IntegratedTest.Tests.WPF;
using Ko.Navigation.CefGlue.Tests.Infra;
using Xunit;

namespace Ko.Navigation.CefGlue.Tests
{
    [Collection("Cef Window Integrated")]
    public class DoubleNavigation_Animation_Cef_Ko_Tests : DoubleNavigation_AnimationTests
    {
        public DoubleNavigation_Animation_Cef_Ko_Tests(CefGlueKoContext context) : base(context)
        {
        }
    }
}
