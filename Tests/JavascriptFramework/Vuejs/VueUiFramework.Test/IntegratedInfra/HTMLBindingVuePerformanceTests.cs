using FluentAssertions;
using Neutronium.Core;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Tests.Universal.HTMLBindingTests;
using Xunit;
using Xunit.Abstractions;
using System;
using MoreCollection.Extensions;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Tests.Universal.HTMLBindingTests.Helper;
using System.Linq;

namespace VueFramework.Test.IntegratedInfra
{
    public abstract class HTMLBindingVuePerformanceTests : HTMLBindingPerformanceTests
    {
        public HTMLBindingVuePerformanceTests(IWindowLessHTMLEngineProvider testEnvironment, ITestOutputHelper output, Dictionary<TestPerformanceKind, int> expectedTimeInMilliSeconds)
            : base(testEnvironment, output, expectedTimeInMilliSeconds)
        {
        }

        public static IEnumerable<object> ObjectSizes =>
                            EnumerableExtensions.Cartesian(
                                new[] { 1, 5, 10, 20, 50 },
                                new[] { 1, 2, 3 }, 
                                (size, rank) => new object[] { size, rank });

        public static IEnumerable<object> IntValues => new[] { 1, 5, 10, 20, 50 }.Select(v => new object[] { v });

        [Theory]
        [MemberData(nameof(ObjectSizes))]
        public async Task Stress_Vm_SynchroneousBuildStrategy(int childrenCount, int rank)
        {
            var strategyBuilder = new ConstJavascriptObjectBuilderStrategyFactory(GetSynchroneousStrategy);
            await Stress_Vm_FromStrategy(childrenCount, rank, strategyBuilder);     
        }

        [Theory]
        [MemberData(nameof(ObjectSizes))]
        public async Task Stress_Vm_BulkBuildStrategy(int childrenCount, int rank)
        {
            var strategyBuilder = new ConstJavascriptObjectBuilderStrategyFactory(GetBulkStrategy);
            await Stress_Vm_FromStrategy(childrenCount, rank, strategyBuilder);
        }

        [Theory]
        [MemberData(nameof(ObjectSizes))]
        public async Task Stress_Vm_MixtBuildStrategy(int childrenCount, int rank)
        {
            var strategyBuilder = new ConstJavascriptObjectBuilderStrategyFactory(GetMixtStrategy);
            await Stress_Vm_FromStrategy(childrenCount, rank, strategyBuilder);
        }

        [Theory]
        [MemberData(nameof(IntValues))]
        public async Task Update_from_int_synchroneous(int value)
        {
            var strategyBuilder = new ConstJavascriptObjectBuilderStrategyFactory(GetSynchroneousStrategy);
            await Update_from_int(value, strategyBuilder);
        }

        [Theory]
        [MemberData(nameof(IntValues))]
        public async Task Update_from_int_Bulk(int value)
        {
            var strategyBuilder = new ConstJavascriptObjectBuilderStrategyFactory(GetBulkStrategy);
            await Update_from_int(value, strategyBuilder);
        }

        [Theory]
        [MemberData(nameof(IntValues))]
        public async Task Update_from_int_Mixt(int value)
        {
            var strategyBuilder = new ConstJavascriptObjectBuilderStrategyFactory(GetMixtStrategy);
            await Update_from_int(value, strategyBuilder);
        }

        public async Task Stress_Vm_FromStrategy(int childrenCount, int rank, IJavascriptObjectBuilderStrategyFactory strategyFactory)
        {
            var root = new FakeFatherViewModel();
            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, root, JavascriptBindingMode.TwoWay, strategyFactory),
                Test = async (mb) =>
                {
                    var bigVm = BuildBigVm(childrenCount, rank);
                    var js = mb.JsRootObject;

                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    await DoSafeAsyncUI(() => root.Other = bigVm);

                    await Task.Delay(_DelayForTimeOut);

                    var other = await _WebView.EvaluateAsync(() => GetAttribute(js, "Other"));

                    stopWatch.Stop();
                    var ts = stopWatch.ElapsedMilliseconds - _DelayForTimeOut;
                    _Logger.Info($"Perf: {((double)(ts)) / 1000} sec");

                    other.IsObject.Should().BeTrue();
                }
            };

            await RunAsync(test);
        }

        public async Task Update_from_int(int value, IJavascriptObjectBuilderStrategyFactory strategyFactory)
        {
            var root = new FakeIntViewModel();
            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, root, JavascriptBindingMode.TwoWay, strategyFactory),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    await DoSafeAsyncUI(() => root.Value = value);

                    await Task.Delay(_DelayForTimeOut);

                    var other = await _WebView.EvaluateAsync(() => GetAttribute(js, "Value"));

                    other.GetIntValue().Should().Be(value);

                    stopWatch.Stop();
                    var ts = stopWatch.ElapsedMilliseconds - _DelayForTimeOut;
                    _Logger.Info($"Perf: {((double)(ts)) / 1000} sec");
                }
            };

            await RunAsync(test);
        }

        private class FakeIntViewModel : ViewModelTestBase
        {
            private int _Value;
            public int Value
            {
                get { return _Value; }
                set { Set(ref _Value, value); }
            }
        }

        private IJavascriptObjectBuilderStrategy GetSynchroneousStrategy(IWebView webView, IJavascriptSessionCache cache, bool mapping)
        {
            return new JavascriptObjectSynchroneousBuilderStrategy(webView, cache, mapping);
        }

        private IJavascriptObjectBuilderStrategy GetBulkStrategy(IWebView webView, IJavascriptSessionCache cache, bool mapping)
        {
            return new JavascriptObjectBulkBuilderStrategy(webView, cache, mapping);
        }

        private IJavascriptObjectBuilderStrategy GetMixtStrategy(IWebView webView, IJavascriptSessionCache cache, bool mapping)
        {
            return new JavascriptObjectMixtBuilderStrategy(webView, cache, mapping);
        }

        private class ConstJavascriptObjectBuilderStrategyFactory : IJavascriptObjectBuilderStrategyFactory
        {
            private readonly Func<IWebView, IJavascriptSessionCache, bool,IJavascriptObjectBuilderStrategy> _StrategyFactory;

            public ConstJavascriptObjectBuilderStrategyFactory(Func<IWebView, IJavascriptSessionCache, bool, IJavascriptObjectBuilderStrategy> strategyFactory)
            {
                _StrategyFactory = strategyFactory;
            }

            public IJavascriptObjectBuilderStrategy GetStrategy(IWebView webView, IJavascriptSessionCache cache, bool mapping)
            {
                return _StrategyFactory(webView, cache, mapping);
            }
        }
    }
}
