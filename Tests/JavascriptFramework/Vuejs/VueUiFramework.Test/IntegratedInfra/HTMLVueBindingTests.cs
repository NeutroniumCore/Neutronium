using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Tests.Universal.HTMLBindingTests;
using Xunit.Abstractions;

namespace VueFramework.Test.IntegratedInfra
{
    public class HTMLVueBindingTests : HTMLBindingTests
    {
        public HTMLVueBindingTests(IWindowLessHTMLEngineProvider context, ITestOutputHelper output)
            : base(context, output)
        {
        }
    }
}
