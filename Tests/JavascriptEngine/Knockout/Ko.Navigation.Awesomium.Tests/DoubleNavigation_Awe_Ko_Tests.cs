using IntegratedTest.Tests.WPF;
using ko.Navigation.Awesomium.Tests.Infra;
using Xunit;

namespace Ko.Binding.Awesomium.Tests
{
    [Collection("Awesomium Window Integrated")]
    public class DoubleNavigation_Awe_Ko_Tests : DoubleNavigationTests
    {
        public DoubleNavigation_Awe_Ko_Tests(AwesomiumKoContext context) : base(context)
        {
        }
    }
}
