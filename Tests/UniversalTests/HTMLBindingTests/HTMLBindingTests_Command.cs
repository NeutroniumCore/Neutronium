using System;
using System.Threading.Tasks;
using System.Windows.Input;
using FluentAssertions;
using Neutronium.Core;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.GlueObject.Executable;
using Neutronium.Core.Test.Helper;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Example.ViewModel;
using Neutronium.MVVMComponents;
using Neutronium.MVVMComponents.Relay;
using NSubstitute;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext;
using Tests.Universal.HTMLBindingTests.Helper;
using Xunit;

namespace Tests.Universal.HTMLBindingTests
{
    public abstract partial class HtmlBindingTests
    {
        [Fact]
        public async Task TwoWay_Command_Basic()
        {
            var command = Substitute.For<ICommand>();
            var datacontexttest = new FakeTestViewModel() { Command = command };

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = ((HtmlBinding)mb).JsBrideRootObject as JsGenericObject;

                    var mycommand = js.GetAttribute("Command") as JsCommand;
                    mycommand.Should().NotBeNull();
                    mycommand.ToString().Should().Be("{}");
                    mycommand.Type.Should().Be(JsCsGlueType.Command);
                    mycommand.CachableJsValue.Should().NotBeNull();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command()
        {
            var command = Substitute.For<ICommand>();
            var datacontexttest = new FakeTestViewModel() { Command = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "Command");
                    DoSafe(() => Call(mycommand, "Execute"));
                    await Task.Delay(100);
                    command.Received().Execute(Arg.Any<object>());
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command_With_Parameter()
        {
            var command = Substitute.For<ICommand>();
            var datacontexttest = new FakeTestViewModel() { Command = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "Command");
                    DoSafe(() => Call(mycommand, "Execute", js));
                    await Task.Delay(100);
                    command.Received().Execute(datacontexttest);
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TwoWay_Command_CanExecute_Set_CanExecuteValue(bool canExecute)
        {
            var command = Substitute.For<ICommand>();
            command.CanExecute(Arg.Any<object>()).Returns(canExecute);
            var datacontexttest = new FakeTestViewModel() { Command = command };

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "Command");
                    bool res = GetBoolAttribute(mycommand, "CanExecuteValue");

                    res.Should().Be(canExecute);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command_Uptate_From_Null()
        {
            var command = Substitute.For<ICommand>();
            command.CanExecute(Arg.Any<object>()).Returns(true);
            var datacontexttest = new FakeTestViewModel();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "Command");
                    mycommand.IsNull.Should().BeTrue();

                    DoSafeUI(() => datacontexttest.Command = command);
                    await Task.Delay(200);

                    mycommand = GetAttribute(js, "Command");
                    DoSafe(() => Call(mycommand, "Execute", js));
                    await Task.Delay(200);
                    command.Received().Execute(datacontexttest);
                }
            };
            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_calls_command_with_correct_argument()
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

                    var mycommand = GetAttribute(js, "Command");
                    DoSafe(() => Call(mycommand, "Execute", childJs));
                    await Task.Delay(300);

                    datacontext.CallCount.Should().Be(1);
                    datacontext.LastCallElement.Should().Be(child);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_CommandWithoutParameter_Is_Mapped()
        {
            var command = Substitute.For<ICommandWithoutParameter>();
            var datacontexttest = new FakeTestViewModel() { CommandWithoutParameters = command };

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = ((HtmlBinding)mb).JsBrideRootObject as JsGenericObject;

                    var mycommand = js.GetAttribute("CommandWithoutParameters") as JsCommandWithoutParameter;
                    mycommand.Should().NotBeNull();
                    mycommand.ToString().Should().Be("{}");
                    mycommand.Type.Should().Be(JsCsGlueType.Command);
                    mycommand.CachableJsValue.Should().NotBeNull();
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TwoWay_CommandWithoutParameter_CanExecuteValue_has_CanExecute_value(bool canExecute)
        {
            var command = Substitute.For<ICommandWithoutParameter>();
            command.CanExecute.Returns(canExecute);
            var datacontexttest = new FakeTestViewModel() { CommandWithoutParameters = command };

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "CommandWithoutParameters");

                    var res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().Be(canExecute);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_CommandWithoutParameter_CanExecute_Refresh_Ok()
        {
            var command = Substitute.For<ICommandWithoutParameter>();
            command.CanExecute.Returns(true);
            var datacontexttest = new FakeTestViewModel() { CommandWithoutParameters = command };

            var test = new TestInContextAsync()
            {
                Path = TestContext.GenericBind,
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "CommandWithoutParameters");
                    var res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().BeTrue();

                    command.CanExecute.Returns(false);

                    DoSafeUI(() => command.CanExecuteChanged += Raise.EventWith(command, new EventArgs()));

                    await Task.Delay(150);

                    mycommand = GetAttribute(js, "CommandWithoutParameters");
                    res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().BeFalse();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_CommandWithoutParameter_Can_Be_Called()
        {
            var command = Substitute.For<ICommandWithoutParameter>();
            var datacontexttest = new FakeTestViewModel() { CommandWithoutParameters = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "CommandWithoutParameters");
                    DoSafe(() => Call(mycommand, "Execute"));
                    await Task.Delay(100);
                    command.Received(1).Execute();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_CommandWithoutParameter_Uptate_From_Null()
        {
            var command = Substitute.For<ICommandWithoutParameter>();
            var datacontexttest = new FakeTestViewModel();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "Command");
                    mycommand.IsNull.Should().BeTrue();

                    DoSafeUI(() => datacontexttest.CommandWithoutParameters = command);
                    await Task.Delay(200);

                    mycommand = GetAttribute(js, "CommandWithoutParameters");
                    DoSafe(() => Call(mycommand, "Execute"));
                    await Task.Delay(200);
                    command.Received().Execute();
                }
            };
            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_CommandGeneric_Is_Mapped()
        {
            var command = Substitute.For<ICommand<string>>();
            var datacontexttest = new FakeTestViewModel() { CommandGeneric = command };

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = ((HtmlBinding)mb).JsBrideRootObject as JsGenericObject;

                    var mycommand = js.GetAttribute("CommandGeneric") as JsCommand<string>;
                    mycommand.Should().NotBeNull();
                    mycommand.ToString().Should().Be("{}");
                    mycommand.Type.Should().Be(JsCsGlueType.Command);
                    mycommand.CachableJsValue.Should().NotBeNull();
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TwoWay_CommandGeneric_CanExecuteValue_default_value_is_true(bool canExecute)
        {
            var command = Substitute.For<ICommand<string>>();
            command.CanExecute(Arg.Any<string>()).Returns(canExecute);
            var datacontexttest = new FakeTestViewModel() { CommandGenericNotBound = command };

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "CommandGenericNotBound");

                    var res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().BeTrue();
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [InlineData("parameter")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("abc")]
        public async Task TwoWay_CommandGeneric_Can_Be_Called_With_Parameter(string parameter)
        {
            var command = Substitute.For<ICommand<string>>();
            var datacontexttest = new FakeTestViewModel() { CommandGeneric = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "CommandGeneric");
                    var jsParameter = (parameter != null) ? _WebView.Factory.CreateString(parameter) : _WebView.Factory.CreateNull();
                    DoSafe(() => Call(mycommand, "Execute", jsParameter));
                    await Task.Delay(150);
                    command.Received(1).Execute(parameter);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_CommandGeneric_Can_Convert_Argument_When_Argument_Mismatch()
        {
            var command = Substitute.For<ICommand<string>>();
            var datacontexttest = new FakeTestViewModel() { CommandGeneric = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "CommandGeneric");
                    DoSafe(() => Call(mycommand, "Execute", _WebView.Factory.CreateInt(10)));
                    await Task.Delay(100);
                    command.Received(1).Execute("10");
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_CommandGeneric_Do_Not_Call_Execute_When_Argument_Can_Not_Be_Converted()
        {
            var command = Substitute.For<ICommand<int>>();
            var datacontexttest = new FakeTestViewModel() { CommandGenericInt = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "CommandGenericInt");
                    DoSafe(() => Call(mycommand, "Execute", _WebView.Factory.CreateString("taDa")));
                    await Task.Delay(100);
                    command.DidNotReceive().Execute(Arg.Any<int>());
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task TwoWay_CommandGeneric_Do_Not_Call_Execute_When_Called_Without_Argument()
        {
            var command = Substitute.For<ICommand<string>>();
            var datacontexttest = new FakeTestViewModel() { CommandGeneric = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "CommandGeneric");
                    DoSafe(() => Call(mycommand, "Execute"));
                    await Task.Delay(100);
                    command.DidNotReceive().Execute(Arg.Any<string>());
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task TwoWay_Command_Generic_Uptate_From_Null()
        {
            var command = Substitute.For<ICommand<string>>();
            var datacontexttest = new FakeTestViewModel();
            var argument = "argument";

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "Command");
                    mycommand.IsNull.Should().BeTrue();

                    DoSafeUI(() => datacontexttest.CommandGeneric = command);
                    await Task.Delay(200);

                    mycommand = GetAttribute(js, "CommandGeneric");
                    DoSafe(() => Call(mycommand, "Execute", _WebView.Factory.CreateString(argument)));
                    await Task.Delay(200);
                    command.Received().Execute(argument);
                }
            };
            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command_Generic_FakeTestViewModel_CanExecute_Refresh_Ok()
        {
            var command = Substitute.For<ICommand<FakeTestViewModel>>();
            command.CanExecute(Arg.Any<FakeTestViewModel>()).Returns(true);
            var datacontexttest = new FakeTestViewModel() { AutoCommand = command };

            var test = new TestInContextAsync()
            {
                Path = TestContext.GenericBind,
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "AutoCommand");
                    var res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().BeTrue();

                    DoSafeUI(() =>
                    {
                        command.CanExecute(Arg.Any<FakeTestViewModel>()).Returns(false);
                        command.CanExecuteChanged += Raise.EventWith(command, new EventArgs());
                    });

                    await Task.Delay(200);

                    mycommand = GetAttribute(js, "AutoCommand");
                    res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().BeFalse();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command_CanExecute_Refresh_Ok()
        {
            bool canexecute = true;
            _Command.CanExecute(Arg.Any<object>()).ReturnsForAnyArgs(x => canexecute);

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "TestCommand");
                    bool res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().BeTrue();

                    canexecute = false;
                    DoSafeUI(() =>
                    {
                        _Command.CanExecuteChanged += Raise.EventWith(_Command, new EventArgs());
                    });

                    await Task.Delay(100);

                    mycommand = GetAttribute(js, "TestCommand");
                    res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    ((bool)res).Should().BeFalse();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command_CanExecute_Refresh_Ok_Argument()
        {
            bool canexecute = true;
            _Command.CanExecute(Arg.Any<object>()).ReturnsForAnyArgs(x => canexecute);

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    await Task.Delay(100);

                    var mycommand = GetAttribute(js, "TestCommand");
                    bool res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().BeTrue();

                    _Command.Received().CanExecute(_DataContext);

                    canexecute = false;
                    _Command.ClearReceivedCalls();

                    DoSafeUI(() =>
                    {
                        _Command.CanExecuteChanged += Raise.EventWith(_Command, new EventArgs());
                    });

                    await Task.Delay(100);

                    _Command.Received().CanExecute(_DataContext);

                    mycommand = GetAttribute(js, "TestCommand");
                    res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().BeFalse();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command_CanExecute_Refresh_Ok_Argument_Exception()
        {
            _Command.CanExecute(Arg.Any<object>()).Returns(x => { if (x[0] == null) throw new Exception(); return false; });

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    _Command.Received().CanExecute(Arg.Any<object>());
                    var js = mb.JsRootObject;

                    await Task.Delay(100);

                    var mycommand = GetAttribute(js, "TestCommand");
                    bool res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().BeFalse();

                    _Command.Received().CanExecute(_DataContext);
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task TwoWay_Command_Received_javascript_variable()
        {
            _Command.CanExecute(Arg.Any<object>()).Returns(true);

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "TestCommand");
                    Call(mycommand, "Execute", _WebView.Factory.CreateString("titi"));

                    await Task.Delay(150);
                    _Command.Received().Execute("titi");
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task TwoWay_Command_Complete()
        {
            _Command = new RelaySimpleCommand(() =>
            {
                _DataContext.MainSkill = new Skill();
                _DataContext.Skills.Add(_DataContext.MainSkill);
            });

            _DataContext.TestCommand = _Command;
            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    _DataContext.Skills.Should().HaveCount(2);

                    DoSafeUI(() =>
                    {
                        _Command.Execute(null);
                    });

                    await Task.Delay(150);

                    var res = GetCollectionAttribute(js, "Skills");

                    res.Should().NotBeNull();
                    res.GetArrayLength().Should().Be(3);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command_With_Null_Parameter()
        {
            var command = Substitute.For<ICommand>();
            var test = new FakeTestViewModel() { Command = command };

            var testR = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, test, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "Command");
                    Call(mycommand, "Execute", _WebView.Factory.CreateNull());

                    await Task.Delay(150);
                    command.Received().Execute(null);
                }
            };

            await RunAsync(testR);
        }

        [Fact]
        public async Task TwoWay_ResultCommand_Received_javascript_variable_and_not_crash_withoutcallback()
        {
            var function = NSubstitute.Substitute.For<Func<int, int>>();
            var dc = new FakeFactory<int, int>(function);

            var test = new TestInContextAsync()
            {
                Path = TestContext.IndexPromise,
                Bind = (win) => HtmlBinding.Bind(win, dc, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "CreateObject");
                    var intValue = _WebView.Factory.CreateInt(25);
                    Call(mycommand, "Execute", intValue);

                    await Task.Delay(700);
                    function.Received(1).Invoke(25);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_ResultCommand_Received_javascript_variable()
        {
            var function = Substitute.For<Func<int, int>>();
            function.Invoke(Arg.Any<int>()).Returns(255);
            var dc = new FakeFactory<int, int>(function);

            var test = new TestInContextAsync()
            {
                Path = TestContext.IndexPromise,
                Bind = (win) => HtmlBinding.Bind(win, dc, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {

                    {
                        var glueobj = (mb as HtmlBinding).JsBrideRootObject as JsGenericObject;
                        var jsResultCommand = glueobj.GetAttribute("CreateObject") as JsResultCommand<int, int>;
                        jsResultCommand.Should().NotBeNull();
                        jsResultCommand.ToString().Should().Be("{}");
                        jsResultCommand.Type.Should().Be(JsCsGlueType.ResultCommand);
                        jsResultCommand.CachableJsValue.Should().NotBeNull();
                    }

                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "CreateObject");

                    var cb = GetCallBackObject();

                    var intValue = _WebView.Factory.CreateInt(25);
                    var resdummy = this.CallWithRes(mycommand, "Execute", intValue, cb);

                    await Task.Delay(100);

                    DoSafeUI(() => function.Received(1).Invoke(25));

                    await Task.Yield();

                    var error = _WebView.GetGlobal().GetValue("err");
                    error.IsUndefined.Should().BeTrue();

                    var resvalue = _WebView.GetGlobal().GetValue("res");
                    int intres = resvalue.GetIntValue();
                    intres.Should().Be(255);
                }
            };

            await RunAsync(test);
        }

        private IJavascriptObject GetCallBackObject()
        {
            var res = _WebView.Eval("(function(){return { fullfill: function (res) {window.res= res; }, reject: function(err){window.err=err;}}; })();", out var cb);
            res.Should().BeTrue();
            cb.Should().NotBeNull();
            cb.IsObject.Should().BeTrue();
            return cb;
        }

        [Fact]
        public async Task TwoWay_ResultCommand_can_be_listened_from_Javascript()
        {
            const string original = "original";
            const string stringExpected = "NewName";
            var result = new SimpleViewModel { Name = original };
            var function = NSubstitute.Substitute.For<Func<int, SimpleViewModel>>();
            function.Invoke(Arg.Any<int>()).Returns(result);

            var dc = new FakeFactory<int, SimpleViewModel>(function);

            var test = new TestInContextAsync()
            {
                Path = TestContext.IndexPromise,
                Bind = (win) => HtmlBinding.Bind(win, dc, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "CreateObject");
                    var cb = GetCallBackObject();

                    var resdummy = this.CallWithRes(mycommand, "Execute", _WebView.Factory.CreateInt(25), cb);

                    await Task.Delay(700);

                    var resvalue = _WebView.GetGlobal().GetValue("res");

                    await Task.Delay(100);

                    var originalValue = GetAttribute(resvalue, nameof(SimpleViewModel.Name)).GetStringValue();

                    originalValue.Should().Be(original);

                    DoSafeUI(() => result.Name = stringExpected);

                    await Task.Delay(100);

                    var newValue = GetAttribute(resvalue, nameof(SimpleViewModel.Name)).GetStringValue();
                    newValue.Should().Be(stringExpected);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_ResultCommand_Without_Argument_Is_Correctly_Mapped()
        {
            var function = Substitute.For<Func<string>>();
            var dc = new FakeFactory<string>(function);

            var test = new TestInContext()
            {
                Path = TestContext.IndexPromise,
                Bind = (win) => HtmlBinding.Bind(win, dc, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var glueobj = (mb as HtmlBinding).JsBrideRootObject as JsGenericObject;
                    var jsResultCommand = glueobj.GetAttribute("CreateObject") as JsResultCommand<string>;
                    jsResultCommand.Should().NotBeNull();
                    jsResultCommand.ToString().Should().Be("{}");
                    jsResultCommand.Type.Should().Be(JsCsGlueType.ResultCommand);
                    jsResultCommand.CachableJsValue.Should().NotBeNull();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_ResultCommand_Without_Argument_returns_result()
        {
            var result = "resultString";
            var function = Substitute.For<Func<string>>();
            function.Invoke().Returns(result);

            var dc = new FakeFactory<string>(function);

            var test = new TestInContextAsync()
            {
                Path = TestContext.IndexPromise,
                Bind = (win) => HtmlBinding.Bind(win, dc, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "CreateObject");
                    var cb = GetCallBackObject();

                    var resdummy = this.CallWithRes(mycommand, "Execute", cb);

                    await Task.Delay(700);

                    var resvalue = _WebView.GetGlobal().GetValue("res");

                    await Task.Delay(100);

                    var originalValue = resvalue.GetStringValue();

                    originalValue.Should().Be(result);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_ResultCommand_can_be_listened_from_CSharp_when_used_in_Vm()
        {
            var child = new BasicTestViewModel();

            var function = Substitute.For<Func<int, BasicTestViewModel>>();
            function.Invoke(Arg.Any<int>()).Returns(child);

            var dataContext = new FactoryFatherTestViewModel(function);

            var test = new TestInContextAsync()
            {
                Path = TestContext.IndexPromise,
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var factory = GetAttribute(js, "Factory");
                    var mycommand = GetAttribute(factory, "CreateObject");
                    var cb = GetCallBackObject();

                    Call(mycommand, "Execute", _WebView.Factory.CreateInt(25), cb);

                    await Task.Delay(200);

                    DoSafeUI(() => { });

                    var resvalue = _WebView.GetGlobal().GetValue("res");

                    var originalValue = GetAttribute(resvalue, nameof(BasicTestViewModel.Value)).GetIntValue();

                    originalValue.Should().Be(-1);

                    SetAttribute(js, nameof(FactoryFatherTestViewModel.Child), resvalue);

                    var res = GetAttribute(js, nameof(FactoryFatherTestViewModel.Child));
                    res.IsObject.Should().BeTrue();

                    await Task.Delay(200);

                    DoSafeUI(() => dataContext.Child.Should().Be(child));

                    var newInt = 45;
                    SetAttribute(resvalue, nameof(BasicTestViewModel.Value), _WebView.Factory.CreateInt(newInt));
                    var updatedValue = GetAttribute(resvalue, nameof(BasicTestViewModel.Value)).GetIntValue();
                    updatedValue.Should().Be(newInt);
                    await Task.Delay(200);
                    DoSafeUI(() => dataContext.Child.Value.Should().Be(newInt));
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_ResultCommand_Received_javascript_variable_should_fault_Onexception()
        {
            var errormessage = "original error message";
            var function = Substitute.For<Func<int, int>>();
            function.When(f => f.Invoke(Arg.Any<int>())).Do(_ => throw new Exception(errormessage));
            var dc = new FakeFactory<int, int>(function);

            var test = new TestInContextAsync()
            {
                Path = TestContext.IndexPromise,
                Bind = (win) => HtmlBinding.Bind(win, dc, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "CreateObject");
                    var res = _WebView.Eval("(function(){return { fullfill: function (res) {window.res=res; }, reject: function(err){window.err=err;}}; })();", out var cb);

                    res.Should().BeTrue();
                    cb.Should().NotBeNull();
                    cb.IsObject.Should().BeTrue();

                    CallWithRes(mycommand, "Execute", _WebView.Factory.CreateInt(25), cb);
                    await Task.Delay(100);
                    DoSafeUI(() => function.Received(1).Invoke(25));

                    await Task.Yield();

                    var error = _WebView.GetGlobal().GetValue("err").GetStringValue();
                    error.Should().Be(errormessage);

                    var resvalue = _WebView.GetGlobal().GetValue("res");
                    resvalue.IsUndefined.Should().BeTrue();
                }
            };

            await RunAsync(test);
        }

        #region SimpleCommand

        [Fact]
        public async Task TwoWay_SimpleCommand_Without_Parameter()
        {
            var command = Substitute.For<ISimpleCommand>();
            var datacontexttest = new SimpleCommandTestViewModel() { SimpleCommandNoArgument = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "SimpleCommandNoArgument");
                    DoSafe(() => Call(mycommand, "Execute"));
                    await Task.Delay(100);
                    command.Received().Execute();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_SimpleCommand_T_With_Parameter()
        {
            var command = Substitute.For<ISimpleCommand<SimpleCommandTestViewModel>>();
            var datacontexttest = new SimpleCommandTestViewModel() { SimpleCommandObjectArgument = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "SimpleCommandObjectArgument");
                    DoSafe(() => Call(mycommand, "Execute", js));
                    await Task.Delay(100);
                    command.Received().Execute(datacontexttest);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_SimpleCommand_T_With_Parameter_Convert_Number_Type()
        {
            var command = Substitute.For<ISimpleCommand<decimal>>();
            var datacontexttest = new SimpleCommandTestViewModel() { SimpleCommandDecimalArgument = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "SimpleCommandDecimalArgument");
                    DoSafe(() => Call(mycommand, "Execute", _WebView.Factory.CreateDouble(0.55)));
                    await Task.Delay(100);
                    command.Received().Execute(0.55m);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_SimpleCommand_T_With_Parameter_Does_Not_Throw_On_Type_Mismatch()
        {
            var command = Substitute.For<ISimpleCommand<decimal>>();
            var datacontexttest = new SimpleCommandTestViewModel() { SimpleCommandDecimalArgument = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "SimpleCommandDecimalArgument");
                    DoSafe(() => Call(mycommand, "Execute", _WebView.Factory.CreateString("u")));
                    await Task.Delay(100);
                    command.DidNotReceive().Execute(Arg.Any<decimal>());
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_SimpleCommand_T_Without_Parameter_Does_not_Throw()
        {
            var command = Substitute.For<ISimpleCommand<SimpleCommandTestViewModel>>();
            var datacontexttest = new SimpleCommandTestViewModel() { SimpleCommandObjectArgument = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "SimpleCommandObjectArgument");
                    DoSafe(() => Call(mycommand, "Execute"));
                    await Task.Delay(100);
                    command.DidNotReceive().Execute(Arg.Any<SimpleCommandTestViewModel>());
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_SimpleCommand_T_Name()
        {
            var command = Substitute.For<ISimpleCommand<SimpleCommandTestViewModel>>();
            var datacontexttest = new SimpleCommandTestViewModel() { SimpleCommandObjectArgument = command };

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = (mb as HtmlBinding).JsBrideRootObject as JsGenericObject;

                    var mysimplecommand = js.GetAttribute("SimpleCommandObjectArgument") as JsSimpleCommand<SimpleCommandTestViewModel>;
                    mysimplecommand.Should().NotBeNull();
                    mysimplecommand.ToString().Should().Be("{}");
                    mysimplecommand.Type.Should().Be(JsCsGlueType.SimpleCommand);
                    mysimplecommand.CachableJsValue.Should().NotBeNull();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_SimpleCommand_Name()
        {
            var command = Substitute.For<ISimpleCommand>();
            var datacontexttest = new SimpleCommandTestViewModel() { SimpleCommandNoArgument = command };

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = (mb as HtmlBinding).JsBrideRootObject as JsGenericObject;

                    var mysimplecommand = js.GetAttribute("SimpleCommandNoArgument") as JsSimpleCommand;
                    mysimplecommand.Should().NotBeNull();
                    mysimplecommand.ToString().Should().Be("{}");
                    mysimplecommand.Type.Should().Be(JsCsGlueType.SimpleCommand);
                    mysimplecommand.CachableJsValue.Should().NotBeNull();
                }
            };

            await RunAsync(test);
        }

        #endregion
    }
}
