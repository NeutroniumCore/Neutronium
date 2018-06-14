using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using FluentAssertions;
using Neutronium.Core;
using Neutronium.Core.Test.Helper;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;
using NSubstitute;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Universal.HTMLBindingTests 
{
    public abstract class BindingLifeCycleTests : HtmlBindingBase
    {
        protected BindingLifeCycleTests(IWindowLessHTMLEngineProvider context, ITestOutputHelper output)
            : base(context, output) {}

        private void CheckReadOnly(IJavascriptObject javascriptObject, bool isReadOnly) 
        {
            var readOnly = GetBoolAttribute(javascriptObject, NeutroniumConstants.ReadOnlyFlag);
            readOnly.Should().Be(isReadOnly);

            CheckHasListener(javascriptObject, !isReadOnly);
        }

        private void CheckHasListener(IJavascriptObject javascriptObject, bool hasListener) 
        {
            var silenterRoot = GetAttribute(javascriptObject, "__silenter");

            if (hasListener)
            {
                silenterRoot.IsObject.Should().BeTrue();
            }
            else 
            {
                silenterRoot.IsUndefined.Should().BeTrue();
            }
        }

        public static IEnumerable<object[]> ReadWriteTestData 
        {
            get 
            {
                yield return new object[] { typeof(ReadOnlyTestViewModel), true };
                yield return new object[] { typeof(ReadWriteTestViewModel), false };
            }
        }

        [Theory]
        [MemberData(nameof(ReadWriteTestData))]
        public async Task TwoWay_should_create_listener_only_for_write_property(Type type, bool readOnly)
        {
            var datacontext = Activator.CreateInstance(type);

            var test = new TestInContext() 
            {
                Bind = (win) => Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = (mb) => 
                {
                    var js = mb.JsRootObject;
                    CheckReadOnly(js, readOnly);
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [MemberData(nameof(ReadWriteTestData))]
        public async Task TwoWay_should_update_from_csharp_readonly_property(Type type, bool readOnly) 
        {
            var datacontext = Activator.CreateInstance(type) as ReadOnlyTestViewModel;

            var test = new TestInContextAsync() 
            {
                Bind = (win) => Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) => 
                {
                    var js = mb.JsRootObject;
                    var newValue = 55;
                    DoSafeUI(() => datacontext.SetReadOnly(newValue));

                    await Task.Delay(150);
                    var readOnlyValue = GetIntAttribute(js, "ReadOnly");

                    readOnlyValue.Should().Be(newValue);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_should_update_from_csharp_readwrite_property() 
        {
            var datacontext = new ReadWriteTestViewModel();

            var test = new TestInContextAsync() 
            {
                Bind = (win) => Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) => 
                {
                    var js = mb.JsRootObject;
                    var newValue = 550;
                    DoSafeUI(() => datacontext.ReadWrite = newValue);

                    await Task.Delay(150);
                    var readOnlyValue = GetIntAttribute(js, "ReadWrite");

                    readOnlyValue.Should().Be(newValue);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_should_update_from_js_readwrite_property() 
        {
            var datacontext = new ReadWriteTestViewModel();

            var test = new TestInContextAsync() 
            {
                Bind = (win) => Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) => 
                {
                    var js = mb.JsRootObject;
                    var newValue = 1200;
                    var jsValue = _WebView.Factory.CreateInt(newValue);
                    SetAttribute(js, "ReadWrite", jsValue);
                    await Task.Delay(150);

                    DoSafeUI(() => datacontext.ReadWrite.Should().Be(newValue));
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [MemberData(nameof(BasicVmData))]
        public async Task TwoWay_should_clean_javascriptObject_listeners_when_object_is_not_part_of_the_graph(BasicTestViewModel remplacementChild) 
        {
            var datacontext = new BasicFatherTestViewModel();
            var child = new BasicTestViewModel();
            datacontext.Child = child;

            var test = new TestInContextAsync() 
            {
                Bind = (win) => Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) => 
                {
                    var js = mb.JsRootObject;
                    var childJs = GetAttribute(js, "Child");

                    CheckReadOnly(childJs, false);

                    DoSafeUI(() => datacontext.Child = remplacementChild);
                    await Task.Delay(150);

                    CheckHasListener(childJs, false);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_should_clean_javascriptObject_listeners_when_object_is_not_part_of_the_graph_js() 
        {
            var datacontext = new BasicFatherTestViewModel();
            var child = new BasicTestViewModel();
            datacontext.Child = child;

            var test = new TestInContextAsync() 
            {
                Bind = (win) => Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) => 
                {
                    var js = mb.JsRootObject;
                    var childJs = GetAttribute(js, "Child");

                    CheckReadOnly(childJs, false);

                    var nullJs = Factory.CreateNull();
                    SetAttribute(js, "Child", nullJs);

                    await Task.Delay(150);

                    DoSafeUI(() => datacontext.Child.Should().BeNull());

                    child.ListenerCount.Should().Be(0);

                    await Task.Delay(100);

                    CheckHasListener(childJs, false);
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [MemberData(nameof(BasicVmData))]
        public async Task TwoWay_should_clean_javascriptObject_listeners_when_object_is_not_part_of_the_graph_array_context(BasicTestViewModel remplacementChild) 
        {
            var datacontext = new BasicListTestViewModel();
            var child = new BasicTestViewModel();
            datacontext.Children.Add(child);

            var test = new TestInContextAsync() 
            {
                Bind = (win) => Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) => 
                {
                    var js = mb.JsRootObject;

                    CheckReadOnly(js, true);

                    var childrenJs = GetCollectionAttribute(js, "Children");
                    var childJs = childrenJs.GetValue(0);

                    CheckReadOnly(childJs, false);

                    DoSafeUI(() => datacontext.Children[0] = remplacementChild);
                    await Task.Delay(150);

                    CheckHasListener(childJs, false);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_should_clean_javascriptObject_listeners_when_object_is_not_part_of_the_graph_array_js_context() 
        {
            var datacontext = new BasicListTestViewModel();
            var child = new BasicTestViewModel();
            datacontext.Children.Add(child);

            var test = new TestInContextAsync() 
            {
                Bind = (win) => Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) => 
                {
                    var js = mb.JsRootObject;

                    CheckReadOnly(js, true);

                    var childrenJs = GetCollectionAttribute(js, "Children");
                    var childJs = childrenJs.GetValue(0);

                    CheckReadOnly(childJs, false);

                    var operabelCollection = GetAttribute(js, "Children");
                    Call(operabelCollection, "pop");

                    await Task.Delay(150);

                    CheckHasListener(childJs, false);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Listens_To_Key_Added_CSharp_Side() 
        {
            dynamic dynamicDataContext = new ExpandoObject();
            dynamicDataContext.ValueInt = 1;

            var test = new TestInContextAsync() 
            {
                Bind = (win) => HtmlBinding.Bind(win, dynamicDataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) => 
                {
                    var js = mb.JsRootObject;

                    var res = GetAttribute(js, "ValueDouble");
                    res.IsUndefined.Should().BeTrue();

                    DoSafeUI(() => { dynamicDataContext.ValueDouble = 0.5; });

                    await Task.Delay(50);

                    var resDouble = GetDoubleAttribute(js, "ValueDouble");
                    resDouble.Should().Be(0.5);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Listens_On_CSharp_Side_To_Keys_Added_On_CSharp_Side() 
        {
            dynamic dynamicDataContext = new ExpandoObject();
            dynamicDataContext.ValueInt = 1;

            var test = new TestInContextAsync() 
            {
                Bind = (win) => HtmlBinding.Bind(win, dynamicDataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) => 
                {
                    var js = mb.JsRootObject;
                    DoSafeUI(() => { dynamicDataContext.ValueDouble = 0.5; });

                    await Task.Delay(50);

                    DoSafeUI(() => { dynamicDataContext.ValueDouble = 23; });

                    await Task.Delay(50);

                    var resDouble = GetDoubleAttribute(js, "ValueDouble");
                    resDouble.Should().Be(23);
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task TwoWay_Command_Generic_String_CanExecute_Refresh_Ok()
        {
            var command = Substitute.For<ICommand<string>>();
            command.CanExecute(Arg.Any<string>()).Returns(true);
            var datacontexttest = new FakeTestViewModel() { CommandGeneric = command };

            var test = new TestInContextAsync()
            {
                Path = TestContext.GenericBind,
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "CommandGeneric");
                    var res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().BeTrue();

                    command.CanExecute(Arg.Any<string>()).Returns(false);
                    command.CanExecuteChanged += Raise.EventWith(_ICommand, new EventArgs());

                    await Task.Delay(100);

                    mycommand = GetAttribute(js, "CommandGeneric");
                    res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().BeFalse();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Listens_On_Js_Side_To_Keys_Added_On_CSharp_Side()
        {
            dynamic dynamicDataContext = new ExpandoObject();
            dynamicDataContext.ValueInt = 1;
            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dynamicDataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    DoSafeUI(() => { dynamicDataContext.ValueDouble = 0.5; });

                    await Task.Delay(50);

                    SetAttribute(js, "ValueDouble", _WebView.Factory.CreateDouble(49));
                    await Task.Delay(50);

                    DoSafeUI(() =>
                    {
                        double value = dynamicDataContext.ValueDouble;
                        value.Should().Be(49);
                    });
                }
            };
            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Listens_To_Key_Added_Js_Side()
        {
            dynamic dynamicDataContext = new ExpandoObject();
            dynamicDataContext.ValueInt = 1;

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dynamicDataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    AddAttribute(js, "ValueDouble", _WebView.Factory.CreateDouble(49));

                    var resDouble = GetDoubleAttribute(js, "ValueDouble");
                    resDouble.Should().Be(49);

                    await Task.Delay(50);

                    double value = dynamicDataContext.ValueDouble;
                    value.Should().Be(49);
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task TwoWay_Listens_On_CSharp_Side_To_Keys_Added_On_Js_Side()
        {
            dynamic dynamicDataContext = new ExpandoObject();
            dynamicDataContext.ValueInt = 1;

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dynamicDataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    AddAttribute(js, "ValueDouble", _WebView.Factory.CreateDouble(49));

                    DoSafeUI(() =>
                    {
                        dynamicDataContext.ValueDouble = 659;
                    });

                    await Task.Delay(50);

                    var resDouble = GetDoubleAttribute(js, "ValueDouble");
                    resDouble.Should().Be(659);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Listens_On_Js_Side_To_Keys_Added_On_Js_Side()
        {
            dynamic dynamicDataContext = new ExpandoObject();
            dynamicDataContext.ValueInt = 1;

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dynamicDataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    AddAttribute(js, "ValueDouble", _WebView.Factory.CreateDouble(49));

                    await Task.Delay(50);

                    SetAttribute(js, "ValueDouble", _WebView.Factory.CreateDouble(7));

                    await Task.Delay(50);

                    DoSafeUI(() =>
                    {
                        double value = dynamicDataContext.ValueDouble;
                        value.Should().Be(7);
                    });
                }
            };

            await RunAsync(test);
        }
    }
}
