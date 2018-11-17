using FluentAssertions;
using MoreCollection.Extensions;
using Neutronium.Core;
using Neutronium.Core.Test.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neutronium.Example.ViewModel;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Xunit;

namespace Tests.Universal.HTMLBindingTests
{
    public abstract partial class HtmlBindingTests
    {
        [Fact]
        public async Task TwoWay_Listens_to_property_update_during_property_changes_update_from_js()
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

                    SetAttribute(js, "Property1", _WebView.Factory.CreateString("a"));

                    await Task.Delay(100);

                    res = GetStringAttribute(js, "Property1");
                    res.Should().Be("a");

                    res = GetStringAttribute(js, "Property2");
                    res.Should().Be("a", "Neutronium listen to object during update");
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Listens_to_property_update_during_property_changes_update_from_Csharp()
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

                    DoSafeUI(() =>
                    {
                        dataContext.Property1 = "a";
                    });

                    await Task.Delay(50);

                    res = GetStringAttribute(js, "Property1");
                    res.Should().Be("a");

                    res = GetStringAttribute(js, "Property2");
                    res.Should().Be("a", "Neutronium listen to object during update");
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Listens_to_Nested_Changes_After_Property_Updates_CSharp_Updates()
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

                    DoSafeUI(() =>
                    {
                        _DataContext.Local = local;
                    });

                    await Task.Delay(100);

                    DoSafeUI(() =>
                    {
                        local.City = "Floripa";
                    });

                    await Task.Delay(100);

                    var js = mb.JsRootObject;

                    var jsLocal = GetAttribute(js, "Local");
                    var city = GetStringAttribute(jsLocal, "City");
                    city.Should().Be("Floripa");
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Listens_to_Nested_Changes_After_Property_Updates_Javascript_Updates()
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

                    DoSafeUI(() =>
                    {
                        _DataContext.Local = local;
                    });

                    await Task.Delay(100);

                    var js = mb.JsRootObject;

                    var jsLocal = GetAttribute(js, "Local");

                    var stringName = Create(() => _WebView.Factory.CreateString("Floripa"));
                    SetAttribute(jsLocal, "City", stringName);

                    await Task.Delay(100);

                    _DataContext.Local.City.Should().Be("Floripa");
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [MemberData(nameof(BasicVmData))]
        public async Task TwoWay_unlistens_when_changing_property(BasicTestViewModel remplacementChild)
        {
            var child = new BasicTestViewModel();
            var datacontext = new BasicTestViewModel { Child = child };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    child.ListenerCount.Should().Be(1);

                    DoSafeUI(() => datacontext.Child = remplacementChild);
                    await Task.Delay(300);

                    child.ListenerCount.Should().Be(0);

                    //If still listening to child, this will raise an exception
                    //for changing property on the wrong thread
                    var third = new BasicTestViewModel();
                    Action safe = () => child.Child = third;
                    safe.Should().NotThrow();
                }
            };

            await RunAsync(test);
        }


        [Theory]
        [MemberData(nameof(BasicVmData))]
        public async Task TwoWay_unlistens_when_changing_property_transient_changes(BasicTestViewModel remplacementChild)
        {
            var child = new BasicTestViewModel();
            var datacontext = new BasicTestViewModel { Child = child };
            var tempChild1 = new BasicTestViewModel();
            var tempChild2 = new BasicTestViewModel();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    child.ListenerCount.Should().Be(1);

                    DoSafeUI(() =>
                    {
                        datacontext.Child = tempChild1;
                        datacontext.Child = tempChild2;
                        datacontext.Child = remplacementChild;
                    });
                    await Task.Delay(300);

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
        public async Task OneTime_does_not_listens_to_any_object(ViewModelTestBase datacontext, params ViewModelTestBase[] children)
        {
            var test = new TestInContext()
            {
                Bind = (win) => Bind(win, datacontext, JavascriptBindingMode.OneTime),
                Test = (mb) =>
                {
                    children.ForEach(child => child.ListenerCount.Should().Be(0));
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [MemberData(nameof(CircularData))]
        public async Task TwoWay_listens_only_once_to_any_object(ViewModelTestBase datacontext, params ViewModelTestBase[] children)
        {
            var test = new TestInContext()
            {
                Bind = (win) => Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    children.ForEach(child => child.ListenerCount.Should().Be(1));
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [MemberData(nameof(CircularData))]
        public async Task TwoWay_unlistens_after_dipose(ViewModelTestBase datacontext, params ViewModelTestBase[] children)
        {
            var test = new TestInContext()
            {
                Bind = (win) => Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    mb.Dispose();
                    children.ForEach(child => child.ListenerCount.Should().Be(0));
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [MemberData(nameof(BasicVmData))]
        public async Task TwoWay_cleans_javascriptObject_cache_when_object_is_not_part_of_the_graph(BasicTestViewModel remplacementChild)
        {
            var datacontext = new BasicFatherTestViewModel();
            var child = new BasicTestViewModel();
            datacontext.Child = child;

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var childJs = GetAttribute(js, "Child");

                    DoSafeUI(() => datacontext.Child = remplacementChild);
                    await Task.Delay(300);

                    var mycommand = GetAttribute(js, "Command");
                    DoSafe(() => Call(mycommand, "Execute", childJs));
                    await Task.Delay(300);

                    datacontext.CallCount.Should().Be(0);
                    datacontext.LastCallElement.Should().BeNull();
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
                yield return new object[] { circularLong, circularLong, new[] { circularLong }, new[] { circularLong.Child, circularLong.Child.Child } };

                var circularLong2 = BuildLongCircular();
                yield return new object[] { circularLong2, circularLong2.Child, new[] { circularLong2, circularLong2.Child }, new[] { circularLong2.Child.Child } };
            }
        }

        private static BasicTestViewModel BuildLongCircular()
        {
            var circularLong = new BasicTestViewModel { Child = new BasicTestViewModel { Child = new BasicTestViewModel() } };
            circularLong.Child.Child.Child = circularLong;
            return circularLong;
        }

        [Theory]
        [MemberData(nameof(CircularDataBreaker))]
        public async Task TwoWay_unlistens_when_object_is_not_part_of_the_graph_respecting_cycle(BasicTestViewModel root, BasicTestViewModel breaker, BasicTestViewModel[] survivores, BasicTestViewModel[] cleaned)
        {
            var test = new TestInContextAsync()
            {
                Bind = (win) => Bind(win, root, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    await DoSafeAsyncUI(() => breaker.Child = null);

                    survivores.ForEach(sur => sur.ListenerCount.Should().Be(1));
                    cleaned.ForEach(sur => sur.ListenerCount.Should().Be(0));
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [MemberData(nameof(CircularDataBreaker))]
        public async Task TwoWay_unlistens_when_object_is_not_part_of_the_graph_respecting_cycle_transient_changes(BasicTestViewModel root, BasicTestViewModel breaker, BasicTestViewModel[] survivores, BasicTestViewModel[] cleaned)
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

                    survivores.ForEach(sur => sur.ListenerCount.Should().Be(1));
                    cleaned.ForEach(sur => sur.ListenerCount.Should().Be(0));
                    new[] { tempChild1, tempChild2 }.ForEach(sur => sur.ListenerCount.Should().Be(0));
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
        public async Task TwoWay_listens_to_elements_in_list()
        {
            var root = BuildList();
            var list = root.Children.ToList();
            var test = new TestInContext()
            {
                Bind = (win) => Bind(win, root, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    list.ForEach(child => child.ListenerCount.Should().Be(1));
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_unlistens_to_elements_removed_from_list_be_clean()
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
        public async Task TwoWay_unlistens_to_elements_removed_from_list_by_remove()
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

                    await Task.Delay(100);

                    removed.ListenerCount.Should().Be(0);
                    list.Skip(1).ForEach(child => child.ListenerCount.Should().Be(1));
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_unlistens_to_elements_removed_from_list_by_set()
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

                    await Task.Delay(100);

                    removed.ListenerCount.Should().Be(0);
                    list.Skip(1).ForEach(child => child.ListenerCount.Should().Be(1));
                    newChild.ListenerCount.Should().Be(1);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_listens_to_all_changes()
        {
            var child = new BasicTestViewModel();
            var datacontext = new BasicTestViewModel { Child = child };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    DoSafeUI(() => datacontext.Child = null);

                    await Task.Delay(300);

                    var third = new BasicTestViewModel();
                    child.Child = third;

                    DoSafeUI(() => datacontext.Child = child);

                    await Task.Delay(300);

                    DoSafeUI(() => third.Value = 3);

                    await Task.Delay(300);

                    var child1 = GetAttribute(js, "Child");
                    var child2 = GetAttribute(child1, "Child");

                    var value = GetIntAttribute(child2, "Value");
                    value.Should().Be(3);

                    var newvalue = 44;
                    var intJS = _WebView.Factory.CreateInt(newvalue);
                    SetAttribute(child2, "Value", intJS);

                    await Task.Delay(300);

                    DoSafeUI(() =>
                    {
                        third.Value.Should().Be(newvalue);
                    });
                }
            };
            await RunAsync(test);
        }
    }
}