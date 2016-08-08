using System.ComponentModel;

namespace IntegratedTest.JavascriptUIFramework 
{
    public enum TestContext 
    {
        [Description("Navigation data\\index.html")]
        SimpleNavigation,

        [Description("javascript\\navigation_1.html")]
        Navigation1,

        [Description("javascript\\navigation_2.html")]
        Navigation2
    }
}
