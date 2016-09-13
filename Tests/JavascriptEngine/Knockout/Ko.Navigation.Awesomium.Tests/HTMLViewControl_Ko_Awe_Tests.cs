using ko.Navigation.Awesomium.Tests.Infra;
using Tests.Universal.NavigationTests;
using Xunit;

namespace ko.Navigation.Awesomium.Tests
{
    [Collection("Awesomium Window Integrated")]
    public class HTMLViewControl_Ko_Awe_Tests : HTMLViewControlTests
    {
        public HTMLViewControl_Ko_Awe_Tests(AwesomiumKoContext context) : base(context)
        {
        }
    }
}
