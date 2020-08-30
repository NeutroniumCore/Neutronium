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
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext;
using Tests.Universal.HTMLBindingTests.Helper;
using Xunit;

namespace Tests.Universal.HTMLBindingTests
{
    public abstract partial class HtmlBindingTests
    {
        [Fact]
        public async Task TwoWay_Maps_Command()
        {
            var command = Substitute.For<ICommand>();
            var fakeTestViewModel = new FakeTestViewModel() { Command = command };

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, fakeTestViewModel, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = ((HtmlBinding)mb).JsBrideRootObject as JsGenericObject;

                    var jsCommand = js.GetAttribute("Command") as JsCommand;
                    jsCommand.Should().NotBeNull();
                    jsCommand.ToString().Should().Be("{}");
                    jsCommand.Type.Should().Be(JsCsGlueType.Command);
                    jsCommand.CachableJsValue.Should().NotBeNull();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Calls_Command_From_Js()
        {
            var command = Substitute.For<ICommand>();
            var fakeTestViewModel = new FakeTestViewModel() { Command = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, fakeTestViewModel, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var jsCommand = GetAttribute(js, "Command");
                    DoSafe(() => Call(jsCommand, "Execute"));
                    await Task.Delay(100);
                    command.Received().Execute(Arg.Any<object>());
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Call_Command_From_Js_With_Parameter()
        {
            var command = Substitute.For<ICommand>();
            var fakeTestViewModel = new FakeTestViewModel() { Command = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, fakeTestViewModel, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var jsCommand = GetAttribute(js, "Command");
                    DoSafe(() => Call(jsCommand, "Execute", js));
                    await Task.Delay(100);
                    command.Received().Execute(fakeTestViewModel);
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TwoWay_Maps_Command_CanExecute_To_Js(bool canExecute)
        {
            var command = Substitute.For<ICommand>();
            command.CanExecute(Arg.Any<object>()).Returns(canExecute);
            var fakeTestViewModel = new FakeTestViewModel() { Command = command };

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, fakeTestViewModel, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JsRootObject;

                    var jsCommand = GetAttribute(js, "Command");
                    var res = GetBoolAttribute(jsCommand, "CanExecuteValue");
                    res.Should().Be(canExecute);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Updates_Command_From_Null()
        {
            var command = Substitute.For<ICommand>();
            command.CanExecute(Arg.Any<object>()).Returns(true);
            var fakeTestViewModel = new FakeTestViewModel();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, fakeTestViewModel, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var jsCommand = GetAttribute(js, "Command");
                    jsCommand.IsNull.Should().BeTrue();

                    DoSafeUI(() => fakeTestViewModel.Command = command);
                    await Task.Delay(200);

                    jsCommand = GetAttribute(js, "Command");
                    DoSafe(() => Call(jsCommand, "Execute", js));
                    await Task.Delay(200);
                    command.Received().Execute(fakeTestViewModel);
                }
            };
            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Calls_Command_With_Correct_Argument()
        {
            var fakeTestViewModel = new BasicFatherTestViewModel();
            var child = new BasicTestViewModel();
            fakeTestViewModel.Child = child;

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, fakeTestViewModel, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var childJs = GetAttribute(js, "Child");

                    var jsCommand = GetAttribute(js, "Command");
                    DoSafe(() => Call(jsCommand, "Execute", childJs));
                    await Task.Delay(300);

                    fakeTestViewModel.CallCount.Should().Be(1);
                    fakeTestViewModel.LastCallElement.Should().Be(child);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Maps_CommandWithoutParameter()
        {
            var command = Substitute.For<ICommandWithoutParameter>();
            var fakeTestViewModel = new FakeTestViewModel() { CommandWithoutParameters = command };

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, fakeTestViewModel, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = ((HtmlBinding)mb).JsBrideRootObject as JsGenericObject;

                    var jsCommand = js.GetAttribute("CommandWithoutParameters") as JsCommandWithoutParameter;
                    jsCommand.Should().NotBeNull();
                    jsCommand.ToString().Should().Be("{}");
                    jsCommand.Type.Should().Be(JsCsGlueType.Command);
                    jsCommand.CachableJsValue.Should().NotBeNull();
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TwoWay_Maps_CommandWithoutParameter_CanExecuteValue_With_CanExecute(bool canExecute)
        {
            var command = Substitute.For<ICommandWithoutParameter>();
            command.CanExecute.Returns(canExecute);
            var fakeTestViewModel = new FakeTestViewModel() { CommandWithoutParameters = command };

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, fakeTestViewModel, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JsRootObject;

                    var jsCommand = GetAttribute(js, "CommandWithoutParameters");

                    var res = GetBoolAttribute(jsCommand, "CanExecuteValue");
                    res.Should().Be(canExecute);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_CommandWithoutParameter_Is_Updated_When_CanExecute_Changes()
        {
            var command = Substitute.For<ICommandWithoutParameter>();
            command.CanExecute.Returns(true);
            var fakeTestViewModel = new FakeTestViewModel() { CommandWithoutParameters = command };

            var test = new TestInContextAsync()
            {
                Path = TestContext.GenericBind,
                Bind = (win) => HtmlBinding.Bind(win, fakeTestViewModel, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var jsCommand = GetAttribute(js, "CommandWithoutParameters");
                    var res = GetBoolAttribute(jsCommand, "CanExecuteValue");
                    res.Should().BeTrue();

                    command.CanExecute.Returns(false);

                    DoSafeUI(() => command.CanExecuteChanged += Raise.EventWith(command, new EventArgs()));

                    await Task.Delay(150);

                    jsCommand = GetAttribute(js, "CommandWithoutParameters");
                    res = GetBoolAttribute(jsCommand, "CanExecuteValue");
                    res.Should().BeFalse();
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task TwoWay_CommandWithoutParameter_Can_Be_Called()
        {
            var command = Substitute.For<ICommandWithoutParameter>();
            var fakeTestViewModel = new FakeTestViewModel() { CommandWithoutParameters = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, fakeTestViewModel, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var jsCommand = GetAttribute(js, "CommandWithoutParameters");
                    DoSafe(() => Call(jsCommand, "Execute"));
                    await Task.Delay(100);
                    command.Received(1).Execute();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_CommandWithoutParameter_Updates_From_Null()
        {
            var command = Substitute.For<ICommandWithoutParameter>();
            var fakeTestViewModel = new FakeTestViewModel();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, fakeTestViewModel, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var jsCommand = GetAttribute(js, "Command");
                    jsCommand.IsNull.Should().BeTrue();

                    DoSafeUI(() => fakeTestViewModel.CommandWithoutParameters = command);
                    await Task.Delay(200);

                    jsCommand = GetAttribute(js, "CommandWithoutParameters");
                    DoSafe(() => Call(jsCommand, "Execute"));
                    await Task.Delay(200);
                    command.Received().Execute();
                }
            };
            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Maps_CommandGeneric()
        {
            var command = Substitute.For<ICommand<string>>();
            var fakeTestViewModel = new FakeTestViewModel() { CommandGeneric = command };

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, fakeTestViewModel, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = ((HtmlBinding)mb).JsBrideRootObject as JsGenericObject;

                    var jsCommand = js.GetAttribute("CommandGeneric") as JsCommand<string>;
                    jsCommand.Should().NotBeNull();
                    jsCommand.ToString().Should().Be("{}");
                    jsCommand.Type.Should().Be(JsCsGlueType.Command);
                    jsCommand.CachableJsValue.Should().NotBeNull();
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TwoWay_Maps_CommandGeneric_CanExecuteValue_With_CanExecute(bool canExecute)
        {
            var command = Substitute.For<ICommand<string>>();
            command.CanExecute(Arg.Any<string>()).Returns(canExecute);
            var fakeTestViewModel = new FakeTestViewModel() { CommandGenericNotBound = command };

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, fakeTestViewModel, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JsRootObject;

                    var jsCommand = GetAttribute(js, "CommandGenericNotBound");

                    var res = GetBoolAttribute(jsCommand, "CanExecuteValue");
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
            var fakeTestViewModel = new FakeTestViewModel() { CommandGeneric = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, fakeTestViewModel, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var jsCommand = GetAttribute(js, "CommandGeneric");
                    var jsParameter = (parameter != null) ? _WebView.Factory.CreateString(parameter) : _WebView.Factory.CreateNull();
                    DoSafe(() => Call(jsCommand, "Execute", jsParameter));
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
            var fakeTestViewModel = new FakeTestViewModel() { CommandGeneric = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, fakeTestViewModel, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var jsCommand = GetAttribute(js, "CommandGeneric");
                    DoSafe(() => Call(jsCommand, "Execute", _WebView.Factory.CreateInt(10)));
                    await Task.Delay(150);
                    command.Received(1).Execute("10");
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_CommandGeneric_Does_Not_Call_Execute_When_Argument_Can_Not_Be_Converted()
        {
            var command = Substitute.For<ICommand<int>>();
            var fakeTestViewModel = new FakeTestViewModel() { CommandGenericInt = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, fakeTestViewModel, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var jsCommand = GetAttribute(js, "CommandGenericInt");
                    DoSafe(() => Call(jsCommand, "Execute", _WebView.Factory.CreateString("taDa")));
                    await Task.Delay(100);
                    command.DidNotReceive().Execute(Arg.Any<int>());
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_CommandGeneric_Does_Not_Call_Execute_When_Called_Without_Argument()
        {
            var command = Substitute.For<ICommand<string>>();
            var fakeTestViewModel = new FakeTestViewModel() { CommandGeneric = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, fakeTestViewModel, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var jsCommand = GetAttribute(js, "CommandGeneric");
                    DoSafe(() => Call(jsCommand, "Execute"));
                    await Task.Delay(100);
                    command.DidNotReceive().Execute(Arg.Any<string>());
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command_Generic_Updates_From_Null()
        {
            var command = Substitute.For<ICommand<string>>();
            var fakeTestViewModel = new FakeTestViewModel();
            var argument = "argument";

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, fakeTestViewModel, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var jsCommand = GetAttribute(js, "Command");
                    jsCommand.IsNull.Should().BeTrue();

                    DoSafeUI(() => fakeTestViewModel.CommandGeneric = command);
                    await Task.Delay(200);

                    jsCommand = GetAttribute(js, "CommandGeneric");
                    DoSafe(() => Call(jsCommand, "Execute", _WebView.Factory.CreateString(argument)));
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
            var fakeTestViewModel = new FakeTestViewModel() { AutoCommand = command };

            var test = new TestInContextAsync()
            {
                Path = TestContext.GenericBind,
                Bind = (win) => HtmlBinding.Bind(win, fakeTestViewModel, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var jsCommand = GetAttribute(js, "AutoCommand");
                    var res = GetBoolAttribute(jsCommand, "CanExecuteValue");
                    res.Should().BeTrue();

                    DoSafeUI(() =>
                    {
                        command.CanExecute(Arg.Any<FakeTestViewModel>()).Returns(false);
                        command.CanExecuteChanged += Raise.EventWith(command, new EventArgs());
                    });

                    await Task.Delay(200);

                    jsCommand = GetAttribute(js, "AutoCommand");
                    res = GetBoolAttribute(jsCommand, "CanExecuteValue");
                    res.Should().BeFalse();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command_Is_Updated_When_CanExecute_Changes()
        {
            var canExecute = true;
            _Command.CanExecute(Arg.Any<object>()).ReturnsForAnyArgs(x => canExecute);

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var jsCommand = GetAttribute(js, "TestCommand");
                    var res = GetBoolAttribute(jsCommand, "CanExecuteValue");
                    res.Should().BeTrue();

                    canExecute = false;
                    DoSafeUI(() =>
                    {
                        _Command.CanExecuteChanged += Raise.EventWith(_Command, new EventArgs());
                    });

                    await Task.Delay(100);

                    jsCommand = GetAttribute(js, "TestCommand");
                    res = GetBoolAttribute(jsCommand, "CanExecuteValue");
                    ((bool)res).Should().BeFalse();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command_Calls_CanExecute_With_Argument()
        {
            var canExecute = true;
            _Command.CanExecute(Arg.Any<object>()).ReturnsForAnyArgs(x => canExecute);

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    await Task.Delay(100);

                    var jsCommand = GetAttribute(js, "TestCommand");
                    var res = GetBoolAttribute(jsCommand, "CanExecuteValue");
                    res.Should().BeTrue();

                    _Command.Received().CanExecute(_DataContext);

                    canExecute = false;
                    _Command.ClearReceivedCalls();

                    DoSafeUI(() =>
                    {
                        _Command.CanExecuteChanged += Raise.EventWith(_Command, new EventArgs());
                    });

                    await Task.Delay(100);

                    _Command.Received().CanExecute(_DataContext);

                    jsCommand = GetAttribute(js, "TestCommand");
                    res = GetBoolAttribute(jsCommand, "CanExecuteValue");
                    res.Should().BeFalse();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command_Does_Not_Throw_When_Command_Is_Misused()
        {
            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    await Task.Delay(100);

                    DoSafeUI(() =>
                    {
                        _Command.CanExecuteChanged += Raise.Event<EventHandler>(null, new EventArgs());
                    });

                    await Task.Delay(100);

                    await Task.Delay(100);
                }
            };

            Func<Task> @do = () => RunAsync(test);

            await @do.Should().NotThrowAsync();
        }

        [Fact]
        public async Task TwoWay_Command_Does_Not_Throw_When_CanExecute_Throws_exception()
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

                    var jsCommand = GetAttribute(js, "TestCommand");
                    bool res = GetBoolAttribute(jsCommand, "CanExecuteValue");
                    res.Should().BeFalse();

                    _Command.Received().CanExecute(_DataContext);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command_Call_Execute_Mapping_Basic_Javascript_Value()
        {
            _Command.CanExecute(Arg.Any<object>()).Returns(true);

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var jsCommand = GetAttribute(js, "TestCommand");
                    Call(jsCommand, "Execute", _WebView.Factory.CreateString("titi"));

                    await Task.Delay(150);
                    _Command.Received().Execute("titi");
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command_Round_Trip()
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
        public async Task TwoWay_Calls_Command_With_Null_Parameter()
        {
            var command = Substitute.For<ICommand>();
            var test = new FakeTestViewModel() { Command = command };

            var testR = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, test, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var jsCommand = GetAttribute(js, "Command");
                    Call(jsCommand, "Execute", _WebView.Factory.CreateNull());

                    await Task.Delay(150);
                    command.Received().Execute(null);
                }
            };

            await RunAsync(testR);
        }

        [Fact]
        public async Task TwoWay_ResultCommand_Receives_Javascript_Variable_And_Does_Not_Crash_Without_Callback()
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
                    var jsCommand = GetAttribute(js, "CreateObject");
                    var intValue = _WebView.Factory.CreateInt(25);
                    Call(jsCommand, "Execute", intValue);

                    await Task.Delay(700);
                    function.Received(1).Invoke(25);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Calls_ResultCommand_With_Javascript_Variable()
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
                        var glueObject = (mb as HtmlBinding).JsBrideRootObject as JsGenericObject;
                        var jsResultCommand = glueObject.GetAttribute("CreateObject") as JsResultCommand<int, int>;
                        jsResultCommand.Should().NotBeNull();
                        jsResultCommand.ToString().Should().Be("{}");
                        jsResultCommand.Type.Should().Be(JsCsGlueType.ResultCommand);
                        jsResultCommand.CachableJsValue.Should().NotBeNull();
                    }

                    var js = mb.JsRootObject;
                    var jsCommand = GetAttribute(js, "CreateObject");

                    var cb = GetCallBackObject();

                    var intValue = _WebView.Factory.CreateInt(25);
                    this.CallWithRes(jsCommand, "Execute", intValue, cb);

                    await Task.Delay(100);

                    DoSafeUI(() => function.Received(1).Invoke(25));

                    await Task.Yield();

                    var error = _WebView.GetGlobal().GetValue("err");
                    error.IsUndefined.Should().BeTrue();

                    var resValue = _WebView.GetGlobal().GetValue("res");
                    var intResult = resValue.GetIntValue();
                    intResult.Should().Be(255);
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
        public async Task TwoWay_ResultCommand_Can_Be_Listened_From_Javascript()
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
                    var jsCommand = GetAttribute(js, "CreateObject");
                    var cb = GetCallBackObject();

                    this.CallWithRes(jsCommand, "Execute", _WebView.Factory.CreateInt(25), cb);

                    await Task.Delay(700);

                    var resValue = _WebView.GetGlobal().GetValue("res");

                    await Task.Delay(100);

                    var originalValue = GetAttribute(resValue, nameof(SimpleViewModel.Name)).GetStringValue();

                    originalValue.Should().Be(original);

                    DoSafeUI(() => result.Name = stringExpected);

                    await Task.Delay(100);

                    var newValue = GetAttribute(resValue, nameof(SimpleViewModel.Name)).GetStringValue();
                    newValue.Should().Be(stringExpected);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Maps_ResultCommand_Without_Argument()
        {
            var function = Substitute.For<Func<string>>();
            var dc = new FakeFactory<string>(function);

            var test = new TestInContext()
            {
                Path = TestContext.IndexPromise,
                Bind = (win) => HtmlBinding.Bind(win, dc, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var glueObject = (mb as HtmlBinding).JsBrideRootObject as JsGenericObject;
                    var jsResultCommand = glueObject.GetAttribute("CreateObject") as JsResultCommand<string>;
                    jsResultCommand.Should().NotBeNull();
                    jsResultCommand.ToString().Should().Be("{}");
                    jsResultCommand.Type.Should().Be(JsCsGlueType.ResultCommand);
                    jsResultCommand.CachableJsValue.Should().NotBeNull();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_ResultCommand_Without_Argument_Returns_Result()
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
                    var jsCommand = GetAttribute(js, "CreateObject");
                    var cb = GetCallBackObject();

                    this.CallWithRes(jsCommand, "Execute", cb);

                    await Task.Delay(700);

                    var resValue = _WebView.GetGlobal().GetValue("res");

                    await Task.Delay(100);

                    var originalValue = resValue.GetStringValue();

                    originalValue.Should().Be(result);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_ResultCommand_Can_Be_Listened_From_CSharp_When_Used_As_Vm()
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
                    var jsCommand = GetAttribute(factory, "CreateObject");
                    var cb = GetCallBackObject();

                    Call(jsCommand, "Execute", _WebView.Factory.CreateInt(25), cb);

                    await Task.Delay(200);

                    DoSafeUI(() => { });

                    var resValue = _WebView.GetGlobal().GetValue("res");

                    var originalValue = GetAttribute(resValue, nameof(BasicTestViewModel.Value)).GetIntValue();

                    originalValue.Should().Be(-1);

                    SetAttribute(js, nameof(FactoryFatherTestViewModel.Child), resValue);

                    var res = GetAttribute(js, nameof(FactoryFatherTestViewModel.Child));
                    res.IsObject.Should().BeTrue();

                    await Task.Delay(200);

                    DoSafeUI(() => dataContext.Child.Should().Be(child));

                    var newInt = 45;
                    SetAttribute(resValue, nameof(BasicTestViewModel.Value), _WebView.Factory.CreateInt(newInt));
                    var updatedValue = GetAttribute(resValue, nameof(BasicTestViewModel.Value)).GetIntValue();
                    updatedValue.Should().Be(newInt);
                    await Task.Delay(200);
                    DoSafeUI(() => dataContext.Child.Value.Should().Be(newInt));
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_ResultCommand_Transforms_CSharp_Exception_In_Failed_Promise()
        {
            var errorMessage = "original error message";
            var function = Substitute.For<Func<int, int>>();
            function.When(f => f.Invoke(Arg.Any<int>())).Do(_ => throw new Exception(errorMessage));
            var dc = new FakeFactory<int, int>(function);

            var test = new TestInContextAsync()
            {
                Path = TestContext.IndexPromise,
                Bind = (win) => HtmlBinding.Bind(win, dc, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var jsCommand = GetAttribute(js, "CreateObject");
                    var res = _WebView.Eval("(function(){return { fullfill: function (res) {window.res=res; }, reject: function(err){window.err=err;}}; })();", out var cb);

                    res.Should().BeTrue();
                    cb.Should().NotBeNull();
                    cb.IsObject.Should().BeTrue();

                    CallWithRes(jsCommand, "Execute", _WebView.Factory.CreateInt(25), cb);
                    await Task.Delay(100);
                    DoSafeUI(() => function.Received(1).Invoke(25));

                    await Task.Yield();

                    var error = _WebView.GetGlobal().GetValue("err").GetStringValue();
                    error.Should().Be(errorMessage);

                    var resValue = _WebView.GetGlobal().GetValue("res");
                    resValue.IsUndefined.Should().BeTrue();
                }
            };

            await RunAsync(test);
        }

        #region SimpleCommand

        [Fact]
        public async Task TwoWay_Calls_SimpleCommand_Without_Parameter()
        {
            var command = Substitute.For<ISimpleCommand>();
            var simpleCommandTestViewModel = new SimpleCommandTestViewModel() { SimpleCommandNoArgument = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, simpleCommandTestViewModel, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var jsCommand = GetAttribute(js, "SimpleCommandNoArgument");
                    DoSafe(() => Call(jsCommand, "Execute"));
                    await Task.Delay(100);
                    command.Received().Execute();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Calls_SimpleCommand_T_With_Parameter()
        {
            var command = Substitute.For<ISimpleCommand<SimpleCommandTestViewModel>>();
            var simpleCommandTestViewModel = new SimpleCommandTestViewModel() { SimpleCommandObjectArgument = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, simpleCommandTestViewModel, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var jsCommand = GetAttribute(js, "SimpleCommandObjectArgument");
                    DoSafe(() => Call(jsCommand, "Execute", js));
                    await Task.Delay(100);
                    command.Received().Execute(simpleCommandTestViewModel);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Calls_SimpleCommand_T_With_Parameter_Converting_Number_Type()
        {
            var command = Substitute.For<ISimpleCommand<decimal>>();
            var simpleCommandTestViewModel = new SimpleCommandTestViewModel() { SimpleCommandDecimalArgument = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, simpleCommandTestViewModel, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var jsCommand = GetAttribute(js, "SimpleCommandDecimalArgument");
                    DoSafe(() => Call(jsCommand, "Execute", _WebView.Factory.CreateDouble(0.55)));
                    await Task.Delay(100);
                    command.Received().Execute(0.55m);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Does_Not_Throw_On_Type_Mismatch_When_Executing_SimpleCommand_T_With_Parameter_On_Js()
        {
            var command = Substitute.For<ISimpleCommand<decimal>>();
            var simpleCommandTestViewModel = new SimpleCommandTestViewModel() { SimpleCommandDecimalArgument = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, simpleCommandTestViewModel, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var jsCommand = GetAttribute(js, "SimpleCommandDecimalArgument");
                    DoSafe(() => Call(jsCommand, "Execute", _WebView.Factory.CreateString("u")));
                    await Task.Delay(100);
                    command.DidNotReceive().Execute(Arg.Any<decimal>());
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Does_Not_Throw_When_SimpleCommand_T_Is_Called_From_Js_Without_Parameter()
        {
            var command = Substitute.For<ISimpleCommand<SimpleCommandTestViewModel>>();
            var simpleCommandTestViewModel = new SimpleCommandTestViewModel() { SimpleCommandObjectArgument = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, simpleCommandTestViewModel, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var jsCommand = GetAttribute(js, "SimpleCommandObjectArgument");
                    DoSafe(() => Call(jsCommand, "Execute"));
                    await Task.Delay(100);
                    command.DidNotReceive().Execute(Arg.Any<SimpleCommandTestViewModel>());
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Maps_SimpleCommand_T_With_Correct_Name()
        {
            var command = Substitute.For<ISimpleCommand<SimpleCommandTestViewModel>>();
            var simpleCommandTestViewModel = new SimpleCommandTestViewModel() { SimpleCommandObjectArgument = command };

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, simpleCommandTestViewModel, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = (mb as HtmlBinding).JsBrideRootObject as JsGenericObject;

                    var jsCommand = js.GetAttribute("SimpleCommandObjectArgument") as JsSimpleCommand<SimpleCommandTestViewModel>;
                    jsCommand.Should().NotBeNull();
                    jsCommand.ToString().Should().Be("{}");
                    jsCommand.Type.Should().Be(JsCsGlueType.SimpleCommand);
                    jsCommand.CachableJsValue.Should().NotBeNull();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Maps_SimpleCommand_With_Correct_Name()
        {
            var command = Substitute.For<ISimpleCommand>();
            var simpleCommandTestViewModel = new SimpleCommandTestViewModel() { SimpleCommandNoArgument = command };

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, simpleCommandTestViewModel, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = (mb as HtmlBinding).JsBrideRootObject as JsGenericObject;

                    var jsCommand = js.GetAttribute("SimpleCommandNoArgument") as JsSimpleCommand;
                    jsCommand.Should().NotBeNull();
                    jsCommand.ToString().Should().Be("{}");
                    jsCommand.Type.Should().Be(JsCsGlueType.SimpleCommand);
                    jsCommand.CachableJsValue.Should().NotBeNull();
                }
            };

            await RunAsync(test);
        }

        #endregion
    }
}
