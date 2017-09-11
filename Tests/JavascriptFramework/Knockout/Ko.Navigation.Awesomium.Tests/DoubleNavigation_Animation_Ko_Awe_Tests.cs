using ko.Navigation.Awesomium.Tests.Infra;
using Tests.Universal.NavigationTests;
using Xunit;
using Xunit.Abstractions;

namespace Ko.Binding.Awesomium.Tests
{
    [Collection("Awesomium Window Integrated")]
    public class DoubleNavigation_Animation_Ko_Awe_Tests : DoubleNavigation_AnimationTests
    {
        public DoubleNavigation_Animation_Ko_Awe_Tests(AwesomiumKoContext context, ITestOutputHelper testOutputHelper) : 
            base(context, testOutputHelper)
        {
        }
    }
}
