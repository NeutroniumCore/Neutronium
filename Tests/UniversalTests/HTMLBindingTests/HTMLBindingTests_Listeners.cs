using FluentAssertions;
using MoreCollection.Extensions;
using Neutronium.Core;
using Neutronium.Core.Binding;
using Neutronium.Core.Test.Helper;
using Neutronium.Example.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Tests.Universal.HTMLBindingTests.Helper;
using Xunit;

namespace Tests.Universal.HTMLBindingTests
{
    public abstract partial class HtmlBindingTests
    {
        [Fact]
        public async Task TwoWay_Listens_To_Property_Update_During_Property_Changes_Update_From_Js()
        {
            var dataContext = new PropertyUpdatingTestViewModel();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res = GetStringAttribute(js, "Property1");
                    res.Should().Be("1");

                    res = GetStringAttribute(js, "Property2");
                    res.Should().Be("2");

                    await SetAttributeAsync(js, "Property1", _WebView.Factory.CreateString("a"));

                    res = await GetStringAttributeAsync(js, "Property1");
                    res.Should().Be("a");

                    res = GetStringAttribute(js, "Property2");
                    res.Should().Be("a", "Neutronium listen to object during update");
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Listens_To_Property_Update_During_Property_Changes_Update_From_Csharp()
        {
            var dataContext = new PropertyUpdatingTestViewModel();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res = GetStringAttribute(js, "Property1");
                    res.Should().Be("1");

                    res = GetStringAttribute(js, "Property2");
                    res.Should().Be("2");

                    await DoSafeAsyncUI(() => { dataContext.Property1 = "a"; });

                    res = await GetStringAttributeAsync(js, "Property1");
                    res.Should().Be("a");

                    res = GetStringAttribute(js, "Property2");
                    res.Should().Be("a", "Neutronium listen to object during update");
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Listens_To_Nested_Changes_After_Property_Updates_CSharp_Updates()
        {
            _DataContext.MainSkill.Should().BeNull();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var local = new Local
                    {
                        City = "JJC"
                    };

                    await DoSafeAsyncUI(() => { _DataContext.Local = local; });

                    await DoSafeAsyncUI(() => { local.City = "Floripa"; });

                    await WaitAnotherWebContextCycle();

                    var js = mb.JsRootObject;
                    var jsLocal = await GetAttributeAsync(js, "Local");
                    var city = GetStringAttribute(jsLocal, "City");

                    city.Should().Be("Floripa");
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Listens_To_Nested_Changes_After_Property_Updates_Javascript_Updates()
        {
            _DataContext.MainSkill.Should().BeNull();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var local = new Local
                    {
                        City = "JJC"
                    };

                    await DoSafeAsyncUI(() => { _DataContext.Local = local; });

                    var js = mb.JsRootObject;

                    var jsLocal = await GetAttributeAsync(js, nameof(_DataContext.Local));

                    var stringName = Create(() => _WebView.Factory.CreateString("Floripa"));
                    await SetAttributeAsync(jsLocal, "City", stringName);

                    await WaitOneCompleteCycle();

                    await DoSafeAsyncUI(() => _DataContext.Local.City.Should().Be("Floripa"));
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [MemberData(nameof(BasicVmData))]
        public async Task TwoWay_Unlistens_When_Property_Is_Changed(BasicTestViewModel remplacementChild)
        {
            var child = new BasicTestViewModel();
            var dataContext = new BasicTestViewModel { Child = child };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    child.ListenerCount.Should().Be(1);

                    await DoSafeAsyncUI(() => dataContext.Child = remplacementChild);

                    await WaitAnotherWebContextCycle();

                    child.ListenerCount.Should().Be(0);

                    //If still listening to child, this will raise an exception
                    //for changing property on the wrong thread
                    var third = new BasicTestViewModel();
                    Action safe = () => child.Child = third;

                    await DoSafeAsync(() => safe.Should().NotThrow());
                }
            };

            await RunAsync(test);
        }


        [Theory]
        [MemberData(nameof(BasicVmData))]
        public async Task TwoWay_Unlistens_When_Property_Has_Transients_Changes(BasicTestViewModel remplacementChild)
        {
            var child = new BasicTestViewModel();
            var dataContext = new BasicTestViewModel { Child = child };
            var tempChild1 = new BasicTestViewModel();
            var tempChild2 = new BasicTestViewModel();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    child.ListenerCount.Should().Be(1);

                    await DoSafeAsyncUI(() =>
                    {
                        dataContext.Child = tempChild1;
                        dataContext.Child = tempChild2;
                        dataContext.Child = remplacementChild;
                    });

                    await WaitAnotherWebContextCycle();

                    tempChild1.ListenerCount.Should().Be(0);
                    tempChild2.ListenerCount.Should().Be(0);
                    child.ListenerCount.Should().Be(0);
                }
            };

            await RunAsync(test);
        }

        public static IEnumerable<object[]> CircularData
        {
            get
            {
                var root = new BasicTestViewModel { Child = new BasicTestViewModel() };
                yield return new object[] { root, new ViewModelTestBase[] { root, root.Child } };

                var root2 = new BasicTestViewModel { Child = new BasicTestViewModel { Child = new BasicTestViewModel() } };
                yield return new object[] { root2, new ViewModelTestBase[] { root2, root2.Child, root2.Child.Child } };

                var circular1 = new BasicTestViewModel();
                circular1.Child = circular1;

                yield return new object[] { circular1, new ViewModelTestBase[] { circular1 } };

                var circular2 = new BasicTestViewModel { Child = new BasicTestViewModel() };
                circular2.Child.Child = circular2;

                yield return new object[] { circular2, new ViewModelTestBase[] { circular2, circular2.Child } };
            }
        }

        [Theory]
        [MemberData(nameof(CircularData))]
        public async Task OneTime_Does_Not_Listen_To_Any_Object(ViewModelTestBase dataContext,
            params ViewModelTestBase[] children)
        {
            var test = new TestInContext()
            {
                Bind = (win) => Bind(win, dataContext, JavascriptBindingMode.OneTime),
                Test = (mb) => { children.ForEach(child => child.ListenerCount.Should().Be(0)); }
            };

            await RunAsync(test);
        }

        [Theory]
        [MemberData(nameof(CircularData))]
        public async Task TwoWay_Listens_Only_Once_To_Each_Object(ViewModelTestBase dataContext,
            params ViewModelTestBase[] children)
        {
            var test = new TestInContext()
            {
                Bind = (win) => Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = (mb) => { children.ForEach(child => child.ListenerCount.Should().Be(1)); }
            };

            await RunAsync(test);
        }

        [Theory]
        [MemberData(nameof(CircularData))]
        public async Task TwoWay_Unlistens_After_Dispose(ViewModelTestBase dataContext,
            params ViewModelTestBase[] children)
        {
            var test = new TestInContextAsync()
            {
                Bind = (win) => Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    await DoSafeAsyncUI(mb.Dispose);
                    children.ForEach(child => child.ListenerCount.Should().Be(0));
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [MemberData(nameof(BasicVmData))]
        public async Task TwoWay_Cleans_Javascript_Objects_Cache_When_Object_Is_Not_Part_Of_The_Graph(
            BasicTestViewModel replacementChild)
        {
            var dataContext = new BasicFatherTestViewModel();
            var child = new BasicTestViewModel();
            dataContext.Child = child;

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var childJs = GetAttribute(js, "Child");

                    await DoSafeAsyncUI(() => dataContext.Child = replacementChild);

                    await WaitAnotherWebContextCycle();

                    var myCommand = await GetAttributeAsync(js, "Command");
                    await DoSafeAsync(() => Call(myCommand, "Execute", childJs));

                    await DoSafeAsyncUI(() =>
                    {
                        dataContext.CallCount.Should().Be(0);
                        dataContext.LastCallElement.Should().BeNull();
                    });
                }
            };

            await RunAsync(test);
        }

        public static IEnumerable<object[]> CircularDataBreaker
        {
            get
            {
                var auto = new BasicTestViewModel { };
                auto.Child = auto;

                yield return new object[] { auto, auto, new[] { auto }, new BasicTestViewModel[0] };

                var circular = new BasicTestViewModel { Child = new BasicTestViewModel() };
                circular.Child.Child = circular;

                yield return new object[] { circular, circular, new[] { circular }, new[] { circular.Child } };

                var circularLong = BuildLongCircular();
                yield return new object[]
                {
                    circularLong, circularLong, new[] {circularLong},
                    new[] {circularLong.Child, circularLong.Child.Child}
                };

                var circularLong2 = BuildLongCircular();
                yield return new object[]
                {
                    circularLong2, circularLong2.Child, new[] {circularLong2, circularLong2.Child},
                    new[] {circularLong2.Child.Child}
                };
            }
        }

        private static BasicTestViewModel BuildLongCircular()
        {
            var circularLong = new BasicTestViewModel
            { Child = new BasicTestViewModel { Child = new BasicTestViewModel() } };
            circularLong.Child.Child.Child = circularLong;
            return circularLong;
        }

        [Theory]
        [MemberData(nameof(CircularDataBreaker))]
        public async Task TwoWay_Unlistens_When_Object_Is_Not_Part_Of_The_Graph_Respecting_Cycle(
            BasicTestViewModel root, BasicTestViewModel breaker, BasicTestViewModel[] survivores,
            BasicTestViewModel[] cleaned)
        {
            var test = new TestInContextAsync()
            {
                Bind = (win) => Bind(win, root, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    await DoSafeAsyncUI(() => breaker.Child = null);

                    //This block allow to wait for another loop of UI/Js context thread to be executed
                    {
                        var child = await EvaluateSafeUIAsync(() => breaker.Child);
                        child.Should().BeNull();
                    }

                    survivores.ForEach(sur => sur.ListenerCount.Should().Be(1));
                    cleaned.ForEach(sur => sur.ListenerCount.Should().Be(0));
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [MemberData(nameof(CircularDataBreaker))]
        public async Task TwoWay_Unlistens_When_Object_Is_Not_Part_of_The_Graph_Respecting_Cycle_Transient_Changes(
            BasicTestViewModel root, BasicTestViewModel breaker, BasicTestViewModel[] survivors,
            BasicTestViewModel[] cleaned)
        {
            var tempChild1 = new BasicTestViewModel();
            var tempChild2 = new BasicTestViewModel();
            var test = new TestInContextAsync()
            {
                Bind = (win) => Bind(win, root, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    await DoSafeAsyncUI(() =>
                    {
                        breaker.Child = tempChild1;
                        breaker.Child = tempChild2;
                        breaker.Child = null;
                    });

                    survivors.ForEach(sur => sur.ListenerCount.Should().Be(1));
                    cleaned.ForEach(sur => sur.ListenerCount.Should().Be(0));
                    new[] { tempChild1, tempChild2 }.ForEach(sur => sur.ListenerCount.Should().Be(0));
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task TwoWay_Listens_after_set_from_javascript_side()
        {
            var root = new BasicTestTwoChildrenViewModel();
            var child = new BasicTestViewModel();

            root.Child1 = child;

            var test = new TestInContextAsync()
            {
                Bind = (win) => Bind(win, root, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var rootJs = mb.JsRootObject;
                    var childJs = GetAttribute(rootJs, "Child1");
                    await SetAttributeAsync(rootJs, "Child2", childJs);

                    await DoSafeAsyncUI(() =>
                    {
                        root.Child2.Should().Be(child);
                        root.Child1 = null;
                    });

                    child.ListenerCount.Should().Be(1);
                }
            };

            await RunAsync(test);
        }

        private static BasicListTestViewModel BuildList()
        {
            var root = new BasicListTestViewModel
            {
                Children =
                {
                    new BasicTestViewModel(),
                    new BasicTestViewModel(),
                    new BasicTestViewModel()
                }
            };
            return root;
        }

        [Fact]
        public async Task TwoWay_Listens_To_Elements_In_List()
        {
            var root = BuildList();
            var list = root.Children.ToList();
            var test = new TestInContext()
            {
                Bind = (win) => Bind(win, root, JavascriptBindingMode.TwoWay),
                Test = (mb) => { list.ForEach(child => child.ListenerCount.Should().Be(1)); }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Unlistens_To_Elements_Removed_from_List_By_Clean()
        {
            var root = BuildList();
            var list = root.Children.ToList();
            var test = new TestInContextAsync()
            {
                Bind = (win) => Bind(win, root, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    await DoSafeAsyncUI(() => root.Children.Clear());
                    list.ForEach(child => child.ListenerCount.Should().Be(0));
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Unlistens_To_Elements_Removed_From_List_By_Remove()
        {
            var root = BuildList();
            var list = root.Children.ToList();
            var removed = list[0];
            var test = new TestInContextAsync()
            {
                Bind = (win) => Bind(win, root, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    await DoSafeAsyncUI(() => root.Children.RemoveAt(0));

                    removed.ListenerCount.Should().Be(0);
                    list.Skip(1).ForEach(child => child.ListenerCount.Should().Be(1));
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Unlistens_To_Elements_Removed_From_List_By_RangeRemove()
        {
            var root = BuildVmWithRangeCollection();
            var removed = new[] { root.List[1], root.List[2] };
            var remain = new[] { root.List[0], root.List[3] };
            var test = new TestInContextAsync()
            {
                Bind = (win) => Bind(win, root, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    await DoSafeAsyncUI(() => root.List.RemoveRange(1, 2));

                    removed.ForEach(child => child.ListenerCount.Should().Be(0));
                    remain.ForEach(child => child.ListenerCount.Should().Be(1));
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Unlistens_To_Elements_Removed_From_List_By_RangeReplace()
        {
            var root = BuildVmWithRangeCollection();
            var removed = new[] { root.List[1], root.List[2] };
            var remain = new List<BasicTestViewModel> { root.List[0], root.List[3] };
            var test = new TestInContextAsync()
            {
                Bind = (win) => Bind(win, root, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var newOnes = new[]
                    {
                        new BasicTestViewModel(),
                        new BasicTestViewModel(),
                    };
                    remain.AddRange(newOnes);
                    await DoSafeAsyncUI(() => root.List.ReplaceRange(1, 2, newOnes));

                    removed.ForEach(child => child.ListenerCount.Should().Be(0));
                    remain.ForEach(child => child.ListenerCount.Should().Be(1));
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Unlistens_To_Elements_Respect_Added_Element_From_List_By_RangeReplace()
        {
            var root = BuildVmWithRangeCollection();
            var remain = new List<BasicTestViewModel> { root.List[0], root.List[1], root.List[2], root.List[3] };
            var test = new TestInContextAsync()
            {
                Bind = (win) => Bind(win, root, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var oldNewOnes = new[]
                    {
                        root.List[2],
                        root.List[1],
                    };
                    await DoSafeAsyncUI(() => root.List.ReplaceRange(1, 2, oldNewOnes));

                    remain.ForEach(child => child.ListenerCount.Should().Be(1));
                }
            };

            await RunAsync(test);
        }

        private static VmWithRangeCollection<BasicTestViewModel> BuildVmWithRangeCollection()
        {
            var root = new VmWithRangeCollection<BasicTestViewModel>();
            root.List.AddRange(new[]
            {
                new BasicTestViewModel(),
                new BasicTestViewModel(),
                new BasicTestViewModel(),
                new BasicTestViewModel()
            });
            return root;
        }

        [Fact]
        public async Task TwoWay_Unlistens_To_Elements_Removed_From_List_By_Set()
        {
            var root = BuildList();
            var list = root.Children.ToList();
            var removed = list[0];
            var test = new TestInContextAsync()
            {
                Bind = (win) => Bind(win, root, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var newChild = new BasicTestViewModel();
                    await DoSafeAsyncUI(() => root.Children[0] = newChild);

                    removed.ListenerCount.Should().Be(0);
                    list.Skip(1).ForEach(child => child.ListenerCount.Should().Be(1));
                    newChild.ListenerCount.Should().Be(1);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Listens_To_All_Changes()
        {
            var child = new BasicTestViewModel();
            var dataContext = new BasicTestViewModel { Child = child };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    await DoSafeAsyncUI(() => dataContext.Child = null);

                    var third = new BasicTestViewModel();
                    await DoSafeAsyncUI(() => child.Child = third);
                    await DoSafeAsyncUI(() => dataContext.Child = child);
                    await DoSafeAsyncUI(() => third.Value = 3);

                    var child1 = await GetAttributeAsync(js, "Child");
                    var child2 = GetAttribute(child1, "Child");

                    var value = await GetIntAttributeAsync(child2, "Value");
                    value.Should().Be(3);

                    var newValue = 44;
                    var intJS = _WebView.Factory.CreateInt(newValue);
                    await SetAttributeAsync(child2, "Value", intJS);

                    await DoSafeAsyncUI(() => { third.Value.Should().Be(newValue); });
                }
            };
            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Is_Garbage_Collecting_String()
        {
            var stringValue = "batman";
            var stringValue2 = "robin";
            var root = new VmWithTwoStrings(stringValue, null);

            Task CacheShouldHaveOnlyString1(ICSharpToJsCache cSharpToJsCache) =>
                DoSafeAsyncUI(() =>
                {
                    cSharpToJsCache.GetCached(stringValue).Should().NotBeNull();
                    cSharpToJsCache.GetCached(stringValue2).Should().BeNull();
                });

            Task CacheShouldHaveNoString(ICSharpToJsCache cSharpToJsCache) =>
                DoSafeAsyncUI(() =>
                {
                    cSharpToJsCache.GetCached(stringValue).Should().BeNull();
                    cSharpToJsCache.GetCached(stringValue2).Should().BeNull();
                });

            Task CacheShouldHaveBothString(ICSharpToJsCache cSharpToJsCache) =>
                DoSafeAsyncUI(() =>
                {
                    cSharpToJsCache.GetCached(stringValue).Should().NotBeNull();
                    cSharpToJsCache.GetCached(stringValue2).Should().NotBeNull();
                });

            async Task UpdateRoot(Action<VmWithTwoStrings> update) => await DoSafeAsyncUI(() => update(root));

            var test = new TestInContextAsync<BindingInContext>()
            {
                Bind = (win) => BindInContext(win, root, JavascriptBindingMode.TwoWay),
                Test = async (context) =>
                {
                    var cache = context.Cache;

                    await CacheShouldHaveOnlyString1(cache);

                    await UpdateRoot((vm) =>
                    {
                        vm.String2 = stringValue2;
                    });
                    await CacheShouldHaveBothString(cache);

                    await UpdateRoot((vm) =>
                    {
                        vm.String2 = null;
                    });
                    await CacheShouldHaveOnlyString1(cache);

                    await UpdateRoot((vm) =>
                    {
                        vm.String2 = stringValue;
                    });
                    await CacheShouldHaveOnlyString1(cache);

                    await UpdateRoot((vm) =>
                    {
                        vm.String1 = null;
                    });
                    await CacheShouldHaveOnlyString1(cache);

                    await UpdateRoot((vm) =>
                    {
                        vm.String2 = null;
                    });
                    await CacheShouldHaveNoString(cache);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Is_Sharing_Glue_Coming_From_Javascript()
        {
            var stringValue = "batman";
            var dataContext = new VmWithTwoStrings(stringValue, null);

            var test = new TestInContextAsync<BindingInContext>()
            {
                Bind = (win) => BindInContext(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (context) =>
                {
                    var cache = context.Cache;
                    var root = context.Binding.JsRootObject;

                    await DoSafeAsyncUI(() =>
                    {
                        cache.GetCached(stringValue).Should().NotBeNull();
                    });

                    var stringFormJs = Factory.CreateString(stringValue);
                    await SetAttributeAsync(root, nameof(dataContext.String2), stringFormJs);

                    await DoSafeAsyncUI(() =>
                    {
                        cache.GetCached(stringValue).Should().NotBeNull();
                        dataContext.String2.Should().Be(stringValue);
                    });

                    await DoSafeAsyncUI(() =>
                    {
                        dataContext.String1 = null;
                    });

                    await DoSafeAsyncUI(() =>
                    {
                        cache.GetCached(stringValue).Should().NotBeNull();
                        dataContext.String2.Should().Be(stringValue);
                    });
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Cache_Value_From_Javascript()
        {
            var stringValue = "batman";
            var dataContext = new VmWithTwoStrings(null, null);

            var test = new TestInContextAsync<BindingInContext>()
            {
                Bind = (win) => BindInContext(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (context) =>
                {
                    var root = context.Binding.JsRootObject;
                    var stringFormJs = Factory.CreateString(stringValue);
                    await SetAttributeAsync(root, nameof(dataContext.String2), stringFormJs);

                    var cache = context.Cache;

                    await DoSafeAsyncUI(() =>
                    {
                        dataContext.String2.Should().Be(stringValue);
                        cache.GetCached(stringValue).Should().NotBeNull();
                    });

                    await DoSafeAsyncUI(() => dataContext.String1 = stringValue);

                    var string1Value = GetAttribute(root, nameof(dataContext.String1));
                    string1Value.GetStringValue().Should().Be(stringValue);
                }
            };

            await RunAsync(test);
        }
    }
}