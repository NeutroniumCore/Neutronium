using System;
using System.Dynamic;
using System.Threading.Tasks;
using FluentAssertions;
using Neutronium.Core;
using Neutronium.Core.Infra.Reflection;
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

        private void CheckObjectObservability(IJavascriptObject javascriptObject, ObjectObservability objectObservability) 
        {
            var readOnly = (ObjectObservability)GetIntAttribute(javascriptObject, NeutroniumConstants.ReadOnlyFlag);
            readOnly.Should().Be(objectObservability);

            CheckHasListener(javascriptObject, !objectObservability.HasFlag(ObjectObservability.ReadOnly));
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

        [Theory]
        [InlineData(typeof(FakeClass), ObjectObservability.None)]
        [InlineData(typeof(None), ObjectObservability.None)]
        [InlineData(typeof(ReadOnly), ObjectObservability.ReadOnly)]
        [InlineData(typeof(ReadOnlyClass), ObjectObservability.ReadOnly)]
        [InlineData(typeof(Observable), ObjectObservability.Observable)]
        [InlineData(typeof(ReadWriteClassWithNotifyPropertyChanged), ObjectObservability.Observable)]
        [InlineData(typeof(ReadWriteTestViewModel), ObjectObservability.Observable)]
        [InlineData(typeof(ReadOnlyClassWithNotifyPropertyChanged), ObjectObservability.ReadOnlyObservable)]
        [InlineData(typeof(ReadOnlyObservable), ObjectObservability.ReadOnlyObservable)]
        [InlineData(typeof(ReadOnlyTestViewModel), ObjectObservability.ReadOnlyObservable)]
        public async Task TwoWay_Creates_Listener_Only_For_Write_Property(Type type, ObjectObservability expected)
        {
            var dataContext = Activator.CreateInstance(type);

            var test = new TestInContext() 
            {
                Bind = (win) => Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = (mb) => 
                {
                    var js = mb.JsRootObject;
                    CheckObjectObservability(js, expected);
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [InlineData(typeof(ReadOnlyTestViewModel))]
        [InlineData(typeof(ReadWriteTestViewModel))]
        public async Task TwoWay_Updates_From_Csharp_Readonly_Property(Type type) 
        {
            var dataContext = Activator.CreateInstance(type) as ReadOnlyTestViewModel;

            var test = new TestInContextAsync() 
            {
                Bind = (win) => Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) => 
                {
                    var js = mb.JsRootObject;
                    var newValue = 55;
                    DoSafeUI(() => dataContext.SetReadOnly(newValue));

                    await Task.Delay(150);
                    var readOnlyValue = GetIntAttribute(js, "ReadOnly");

                    readOnlyValue.Should().Be(newValue);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Updates_From_Csharp_Readwrite_Property() 
        {
            var dataContext = new ReadWriteTestViewModel();

            var test = new TestInContextAsync() 
            {
                Bind = (win) => Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) => 
                {
                    var js = mb.JsRootObject;
                    var newValue = 550;
                    DoSafeUI(() => dataContext.ReadWrite = newValue);

                    await Task.Delay(150);
                    var readOnlyValue = GetIntAttribute(js, "ReadWrite");

                    readOnlyValue.Should().Be(newValue);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Updates_From_Js_Readwrite_Property() 
        {
            var dataContext = new ReadWriteTestViewModel();

            var test = new TestInContextAsync() 
            {
                Bind = (win) => Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) => 
                {
                    var js = mb.JsRootObject;
                    var newValue = 1200;
                    var jsValue = _WebView.Factory.CreateInt(newValue);
                    SetAttribute(js, "ReadWrite", jsValue);
                    await Task.Delay(150);

                    DoSafeUI(() => dataContext.ReadWrite.Should().Be(newValue));
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [MemberData(nameof(BasicVmData))]
        public async Task TwoWay_Cleans_JavascriptObject_Listeners_When_Object_Is_Not_Part_Of_The_Graph(BasicTestViewModel remplacementChild) 
        {
            var dataContext = new BasicFatherTestViewModel();
            var child = new BasicTestViewModel();
            dataContext.Child = child;

            var test = new TestInContextAsync() 
            {
                Bind = (win) => Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) => 
                {
                    var js = mb.JsRootObject;
                    var childJs = GetAttribute(js, "Child");

                    CheckObjectObservability(childJs, ObjectObservability.Observable);

                    DoSafeUI(() => dataContext.Child = remplacementChild);
                    await Task.Delay(150);

                    CheckHasListener(childJs, false);
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [MemberData(nameof(BasicVmData))]
        public async Task TwoWay_Cleans_JavascriptObject_Listeners_When_Object_Is_Not_Part_Of_The_Graph_Multiple_Changes(BasicTestViewModel remplacementChild)
        {
            var dataContext = new BasicFatherTestViewModel();
            var child = new BasicTestViewModel();
            dataContext.Child = child;

            var tempChild1 = new BasicTestViewModel();
            var tempChild2 = new BasicTestViewModel();

            var test = new TestInContextAsync()
            {
                Bind = (win) => Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var childJs = GetAttribute(js, "Child");

                    CheckObjectObservability(childJs, ObjectObservability.Observable);

                    DoSafeUI(() =>
                    {
                        dataContext.Child = tempChild1;
                        dataContext.Child = tempChild2;
                        dataContext.Child = remplacementChild;
                    });
                    await Task.Delay(150);

                    CheckHasListener(childJs, false);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Cleans_JavascriptObject_Listeners_When_Object_Is_Not_Part_Of_The_Graph_Js() 
        {
            var dataContext = new BasicFatherTestViewModel();
            var child = new BasicTestViewModel();
            dataContext.Child = child;

            var test = new TestInContextAsync() 
            {
                Bind = (win) => Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) => 
                {
                    var js = mb.JsRootObject;
                    var childJs = GetAttribute(js, "Child");

                    CheckObjectObservability(childJs, ObjectObservability.Observable);

                    var nullJs = Factory.CreateNull();
                    SetAttribute(js, "Child", nullJs);

                    await Task.Delay(150);

                    DoSafeUI(() => dataContext.Child.Should().BeNull());

                    child.ListenerCount.Should().Be(0);

                    await Task.Delay(100);

                    CheckHasListener(childJs, false);
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [MemberData(nameof(BasicVmData))]
        public async Task TwoWay_Cleans_JavascriptObject_Listeners_When_Object_Is_Not_Part_Of_The_Graph_Array_Context(BasicTestViewModel remplacementChild) 
        {
            var dataContext = new BasicListTestViewModel();
            var child = new BasicTestViewModel();
            dataContext.Children.Add(child);

            var test = new TestInContextAsync() 
            {
                Bind = (win) => Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) => 
                {
                    var js = mb.JsRootObject;

                    CheckObjectObservability(js, ObjectObservability.ReadOnlyObservable);

                    var childrenJs = GetCollectionAttribute(js, "Children");
                    var childJs = childrenJs.GetValue(0);

                    CheckObjectObservability(childJs, ObjectObservability.Observable);

                    DoSafeUI(() => dataContext.Children[0] = remplacementChild);
                    await Task.Delay(150);

                    CheckHasListener(childJs, false);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Cleans_JavascriptObject_Listeners_When_Object_Is_Not_Part_Of_The_Graph_Array_Js_Context() 
        {
            var dataContext = new BasicListTestViewModel();
            var child = new BasicTestViewModel();
            dataContext.Children.Add(child);

            var test = new TestInContextAsync() 
            {
                Bind = (win) => Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) => 
                {
                    var js = mb.JsRootObject;

                    CheckObjectObservability(js, ObjectObservability.ReadOnlyObservable);

                    var childrenJs = GetCollectionAttribute(js, "Children");
                    var childJs = childrenJs.GetValue(0);

                    CheckObjectObservability(childJs, ObjectObservability.Observable);

                    var observableCollection = GetAttribute(js, "Children");
                    Call(observableCollection, "pop");

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

                    DoSafeUI(() =>
                    {
                        dynamicDataContext.ValueDouble = 0.5;
                    });

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
                    DoSafeUI(() => command.CanExecuteChanged += Raise.EventWith(command, new EventArgs()));

                    await Task.Delay(100);

                    mycommand = GetAttribute(js, "CommandGeneric");
                    res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().BeFalse();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command_Generic_String_Maps_As_ObservableReadOnly()
        {
            var command = Substitute.For<ICommand<string>>();
            command.CanExecute(Arg.Any<string>()).Returns(true);
            var datacontexttest = new FakeTestViewModel() { CommandGeneric = command };

            var test = new TestInContext()
            {
                Path = TestContext.GenericBind,
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "CommandGeneric");
                    CheckObjectObservability(mycommand, ObjectObservability.ReadOnlyObservable);               
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_SimpleCommand_Maps_As_ReadOnly()
        {
            var command = Substitute.For<ISimpleCommand<SimpleCommandTestViewModel>>();
            var datacontexttest = new SimpleCommandTestViewModel() { SimpleCommandObjectArgument = command };

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "SimpleCommandObjectArgument");
                    CheckObjectObservability(mycommand, ObjectObservability.ReadOnly);
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
