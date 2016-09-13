using Ko.Navigation.CefGlue.Tests.Infra;
using Tests.Universal.NavigationTests;
using Xunit;

namespace Ko.Navigation.CefGlue.Tests
{
    [Collection("Cef Window Integrated")]
    public class DoubleNavigation_Animation_Ko_Cef_Tests : DoubleNavigation_AnimationTests
    {
        public DoubleNavigation_Animation_Ko_Cef_Tests(CefGlueKoContext context) : base(context)
        {
        }
    }
}
