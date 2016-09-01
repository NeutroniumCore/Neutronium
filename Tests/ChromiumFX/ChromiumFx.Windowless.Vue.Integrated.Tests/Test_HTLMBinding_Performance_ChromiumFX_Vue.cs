using System.Collections.Generic;
using ChromiumFx.Windowless.Vue.Integrated.Tests;
using IntegratedTest.Tests.Windowless;
using Xunit;
using Xunit.Abstractions;

namespace ChromiumFX.Windowless.Integrated.Tests
{
    [Collection("ChromiumFX Vue Windowless Integrated")]
    public class Test_HTLMBinding_Performance_ChromiumFX_Vue : Test_HTLMBinding_Performance
    {
        public Test_HTLMBinding_Performance_ChromiumFX_Vue(ChromiumFXWindowLessHTMLEngineProviderVue context, ITestOutputHelper output) : 
            base(context, output, GetKoCefPerformanceExpectations())
        {
            context.Logger = _Logger;
        }

        private static Dictionary<TestPerformanceKind, int> GetKoCefPerformanceExpectations() 
        {
            return new Dictionary<TestPerformanceKind, int>
            {
                { TestPerformanceKind.OneTime_Collection_CreateBinding, 1500},           
                { TestPerformanceKind.OneWay_Collection_CreateBinding, 1500},
                { TestPerformanceKind.TwoWay_Collection_CreateBinding, 1700},
                { TestPerformanceKind.TwoWay_Int, 3100},
                { TestPerformanceKind.TwoWay_Collection, 4700}
            };
        }
    }
}
