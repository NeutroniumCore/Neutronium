using Ko.Binding.CefGlue.Tests.Infra;
using System.Collections.Generic;
using Tests.Universal.HTMLBindingTests;
using Xunit;
using Xunit.Abstractions;

namespace Ko.Binding.Awesomium.Tests
{
    [Collection("Cef Ko Windowless Integrated")]
    public class BindingPerformance_Ko_Cef_Tests : HTLMBindingPerformanceTests
    {
        public BindingPerformance_Ko_Cef_Tests(CefGlueKoContext context, ITestOutputHelper output)
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
                {TestPerformanceKind.TwoWay_Collection, 4700}
            };
        }
    }
}
