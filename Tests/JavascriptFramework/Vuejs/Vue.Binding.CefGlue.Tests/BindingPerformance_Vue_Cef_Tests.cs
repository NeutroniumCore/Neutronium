//using Vue.Binding.CefGlue.Tests.Infra;
//using System.Collections.Generic;
//using Tests.Universal.HTMLBindingTests;
//using Xunit;
//using Xunit.Abstractions;
//using VueFramework.Test.IntegratedInfra;

//namespace Vue.Binding.CefGlue.Tests
//{
//    [Collection("Cef Vue Windowless Integrated")]
//    public class BindingPerformance_Vue_Cef_Tests : HTMLBindingVuePerformanceTests
//    {
//        public BindingPerformance_Vue_Cef_Tests(CefGlueVueContext context, ITestOutputHelper output)
//            : base(context, output, GetKoCefPerformanceExpectations())
//        {
//        }

//        private static Dictionary<TestPerformanceKind, int> GetKoCefPerformanceExpectations()
//        {
//            return new Dictionary<TestPerformanceKind, int>
//            {
//                {TestPerformanceKind.OneTime_Collection_CreateBinding, 1500},
//                {TestPerformanceKind.TwoWay_Collection_CreateBinding, 1500},
//                {TestPerformanceKind.OneWay_Collection_CreateBinding, 1500},
//                {TestPerformanceKind.TwoWay_Int, 3100},
//                {TestPerformanceKind.TwoWay_Collection, 4700},
//                {TestPerformanceKind.TwoWay_Collection_Update_From_Javascript, 150}
//            };
//        }
//    }
//}
