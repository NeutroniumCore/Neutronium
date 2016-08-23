using System.Collections.Generic;
using IntegratedTest.Tests.Windowless;
using Xunit;
using Xunit.Abstractions;

namespace CefGlue.Windowless.Integrated.Ko.Tests
{
    [Collection("Cef Windowless Ko Integrated")]
    public class Test_HTLMBinding_Performance_Cef_Ko : Test_HTLMBinding_Performance
    {
        public Test_HTLMBinding_Performance_Cef_Ko(CefGlueWindowlessSharedJavascriptEngineFactoryKo context, ITestOutputHelper output) : 
            base(context, output, GetKoCefPerformanceExpectations())
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
