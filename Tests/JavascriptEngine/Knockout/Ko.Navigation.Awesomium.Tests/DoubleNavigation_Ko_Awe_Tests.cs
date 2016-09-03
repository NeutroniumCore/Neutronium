using IntegratedTest.Tests.WPF;
using ko.Navigation.Awesomium.Tests.Infra;
using Xunit;

namespace Ko.Binding.Awesomium.Tests
{
    [Collection("Awesomium Window Integrated")]
    public class DoubleNavigation_Ko_Awe_Tests : DoubleNavigationTests
    {
        public DoubleNavigation_Ko_Awe_Tests(AwesomiumKoContext context) : base(context)
        {
        }
    }
}
