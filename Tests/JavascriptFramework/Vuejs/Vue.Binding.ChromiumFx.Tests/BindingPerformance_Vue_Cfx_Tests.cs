using Vue.Binding.ChromiumFx.Tests.Infra;
using System.Collections.Generic;
using Tests.Universal.HTMLBindingTests;
using Xunit;
using Xunit.Abstractions;

namespace Vue.Binding.ChromiumFx.Tests
{
    [Collection("Cfx Vue Windowless Integrated")]
    public class BindingPerformance_Vue_Cfx_Tests : HTLMBindingPerformanceTests
    {
        public BindingPerformance_Vue_Cfx_Tests(CfxVueContext context, ITestOutputHelper output)
            : base(context, output, GetKoCefPerformanceExpectations())
        {
        }

        private static Dictionary<TestPerformanceKind, int> GetKoCefPerformanceExpectations()
        {
            return new Dictionary<TestPerformanceKind, int>
            {
                {TestPerformanceKind.OneTime_Collection_CreateBinding, 1500},
                {TestPerformanceKind.TwoWay_Collection_CreateBinding, 1500},
                {TestPerformanceKind.OneWay_Collection_CreateBinding, 1500},
                {TestPerformanceKind.TwoWay_Int, 3100},
                {TestPerformanceKind.TwoWay_Collection, 4700},
                {TestPerformanceKind.TwoWay_Collection_Update_From_Javascript, 150}
            };
        }
    }
}
