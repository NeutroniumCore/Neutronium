using IntegratedTest.Tests.WPF;
using ko.Navigation.Awesomium.Tests.Infra;
using Xunit;

namespace Ko.Binding.Awesomium.Tests
{
    [Collection("Awesomium Window Integrated")]
    public class DoubleNavigation_Animation_Ko_Awe_Tests : DoubleNavigation_AnimationTests
    {
        public DoubleNavigation_Animation_Ko_Awe_Tests(AwesomiumKoContext context) : base(context)
        {
        }
    }
}
