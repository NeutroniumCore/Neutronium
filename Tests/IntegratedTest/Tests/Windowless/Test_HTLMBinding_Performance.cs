using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MVVM.HTML.Core;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.ViewModel.Example;
using Tests.Infra.HTMLEngineTesterHelper.HtmlContext;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Xunit;
using Xunit.Abstractions;

namespace IntegratedTest.Tests.Windowless 
{
    public abstract class Test_HTLMBinding_Performance : Test_HTMLBinding_Base 
    {
        private readonly Dictionary<TestPerformanceKind, int> _ExpectedTimeInMilliSeconds; 
        protected Test_HTLMBinding_Performance(IWindowLessHTMLEngineProvider testEnvironment, ITestOutputHelper output, Dictionary<TestPerformanceKind, int> expectedTimeInMilliSeconds) : base(testEnvironment, output)
        {
            _ExpectedTimeInMilliSeconds = expectedTimeInMilliSeconds;
        }

        private class TwoList 
        {
            public TwoList() 
            {
                L1 = new List<Skill>();
                L2 = new List<Skill>();
            }
            public List<Skill> L1 { get; }
            public List<Skill> L2 { get; }
        }

        private void CheckVsExpectation(long value, TestPerformanceKind context) 
        {           
            var expected = GetExpected(context);
            _Logger.Info($"Time expectation for the task: {expected}");
            TimeSpan.FromMilliseconds(value).Should().BeLessThan(expected);
        }

        private TimeSpan GetExpected(TestPerformanceKind context) 
        {
            return TimeSpan.FromMilliseconds(_ExpectedTimeInMilliSeconds[context]);  
        }

        [Fact]
        public Task Test_HTMLBinding_Stress_TwoWay_Collection_CreateBinding() 
        {
            return Test_HTMLBinding_Stress_Collection_CreateBinding(JavascriptBindingMode.TwoWay, TestPerformanceKind.TwoWay_Collection_CreateBinding, TestContext.Simple);
        }

        [Fact]
        public Task Test_HTMLBinding_Stress_OneWay_Collection_CreateBinding() 
        {
            return Test_HTMLBinding_Stress_Collection_CreateBinding(JavascriptBindingMode.OneWay, TestPerformanceKind.OneWay_Collection_CreateBinding, TestContext.Simple);
        }

        [Fact]
        public Task Test_HTMLBinding_Stress_OneTime_Collection_CreateBinding() 
        {
            return Test_HTMLBinding_Stress_Collection_CreateBinding(JavascriptBindingMode.OneTime, TestPerformanceKind.OneTime_Collection_CreateBinding, TestContext.Simple);
        }

        public Task Test_HTMLBinding_Stress_Collection_CreateBinding(JavascriptBindingMode imode, TestPerformanceKind context, TestContext ipath = TestContext.Index) 
        {
            int r = 100;
            var datacontext = new TwoList();
            datacontext.L1.AddRange(Enumerable.Range(0, r).Select(i => new Skill()));

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var test = new TestInContext() 
            {
                Path = ipath,
                Bind = (win) => HTML_Binding.Bind(win, datacontext, imode),
                Test = (mb) => 
                {
                    stopWatch.Stop();
                    var ts = stopWatch.ElapsedMilliseconds;
                    _Logger.Info($"Perf: {((double) (ts)) / 1000} sec for {r} iterations");

                    var js = mb.JSRootObject;

                    var col = GetSafe(() => GetCollectionAttribute(js, "L1"));
                    col.GetArrayLength().Should().Be(r);

                    CheckVsExpectation(ts, context);
                }
            };
            return RunAsync(test);
        }

        [Fact]
        public async Task Bind_ShouldBeRobust() 
        {
            var test = new TestInContext() 
            {
                Bind = (win) => HTML_Binding.Bind(win,  _DataContext, JavascriptBindingMode.TwoWay),
                Test = _ =>  { }
            };

            for (var i = 0; i < 150; i++) 
            {
                _Logger.Info($"Runing interaction {i}");
                 await RunAsync(test);
            }    
        }

        [Fact]
        public async Task Test_HTMLBinding_Stress_Collection_Update_From_Javascript() 
        {
            int r = 100;
            var datacontext = new TwoList();
            datacontext.L1.AddRange(Enumerable.Range(0, r).Select(i => new Skill()));

            var test = new TestInContextAsync() 
            {
                Path = TestContext.Simple,
                Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) => 
                {
                    var js = mb.JSRootObject;

                    var col1 = GetCollectionAttribute(js, "L1");
                    col1.GetArrayLength().Should().Be(r);

                    var col2 = GetCollectionAttribute(js, "L2");
                    col2.GetArrayLength().Should().Be(0);

                    var l2c = GetAttribute(js, "L2");
                    l2c.Should().NotBeNull();

                    var javascript = "window.app = function(value,coll){var args = []; args.push(0); args.push(0); for (var i = 0; i < value.length; i++) { args.push(value[i]);} coll.splice.apply(coll, args);  console.log(value.length); console.log(coll.length);};";
                    IJavascriptObject res = null;
                    bool ok = _WebView.Eval(javascript, out res);
                    ok.Should().BeTrue();

                    bool notok = true;
                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    DoSafe(() => Call(_WebView.GetGlobal(), "app", col1, l2c));
                    while (notok) 
                    {
                        await Task.Delay(100);
                        notok = datacontext.L2.Count != r;
                    }
                    stopWatch.Stop();
                    long ts = stopWatch.ElapsedMilliseconds;

                    _Logger.Info($"Perf: {((double) (ts)) / 1000} sec for {r} iterations");
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task Test_HTMLBinding_Stress_TwoWay_Int() 
        {
            var test = new TestInContextAsync() 
            {
                Path = TestContext.Simple,
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) => 
                {
                    var js = mb.JSRootObject;
                    int iis = 500;
                    for (int i = 0; i < iis; i++) 
                    {
                        _DataContext.Age += 1;
                    }

                    bool notok = true;
                    var tg = _DataContext.Age;
                    await Task.Delay(700);

                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    while (notok) 
                    {
                        await Task.Delay(100);
                        var doublev = GetIntAttribute(js, "Age");
                        notok = doublev != tg;
                    }
                    stopWatch.Stop();
                    var ts = stopWatch.ElapsedMilliseconds;
                    _Logger.Info($"Perf: {((double) (ts)) / 1000} sec for {iis} iterations");

                    CheckVsExpectation(ts, TestPerformanceKind.TwoWay_Int);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task Test_HTMLBinding_Stress_TwoWay_Collection() 
        {

            var test = new TestInContextAsync() 
            {
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) => 
                {
                    var js = mb.JSRootObject;

                    var col = GetSafe(() => GetCollectionAttribute(js, "Skills"));
                    col.GetArrayLength().Should().Be(2);

                    Check(col, _DataContext.Skills);

                    _DataContext.Skills.Add(new Skill() { Name = "C++", Type = "Info" });

                    await Task.Delay(150);
                    col = GetSafe(() => GetCollectionAttribute(js, "Skills"));
                    Check(col, _DataContext.Skills);

                    _DataContext.Skills[0] = new Skill() { Name = "HTML5", Type = "Info" };
                    int iis = 500;
                    for (int i = 0; i < iis; i++) 
                    {
                        _DataContext.Skills.Insert(0, new Skill() { Name = "HTML5", Type = "Info" });
                    }

                    bool notok = true;
                    int tcount = _DataContext.Skills.Count;

                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    while (notok) 
                    {
                        await Task.Delay(10);
                        col = GetSafe(() => GetCollectionAttribute(js, "Skills"));
                        notok = col.GetArrayLength() != tcount;
                    }
                    stopWatch.Stop();
                    var ts = stopWatch.ElapsedMilliseconds;
                    _Logger.Info($"Perf: {((double) (ts)) / 1000} sec for {iis} iterations");
                    Check(col, _DataContext.Skills);

                    CheckVsExpectation(ts, TestPerformanceKind.TwoWay_Collection);
                }
            };

            await RunAsync(test);
        }
    }
}
