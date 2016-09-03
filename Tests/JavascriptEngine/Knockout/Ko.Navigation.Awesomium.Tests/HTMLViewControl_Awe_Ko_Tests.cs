using IntegratedTest.Tests.WPF;
using ko.Navigation.Awesomium.Tests.Infra;
using Xunit;

namespace Ko.Binding.Awesomium.Tests
{
    [Collection("Awesomium Window Integrated")]
    public class HTMLViewControl_Awe_Ko_Tests : HTMLViewControlTests
    {
        public HTMLViewControl_Awe_Ko_Tests(AwesomiumKoContext context) : base(context)
        {
        }
    }
}
