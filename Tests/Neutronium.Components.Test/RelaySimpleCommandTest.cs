using System;
using System.Threading.Tasks;
using FluentAssertions;
using Neutronium.MVVMComponents.Relay;
using NSubstitute;
using Xunit;

namespace Neutronium.Components.Tests
{
    public class RelaySimpleCommandTest
    {
        private Action _Action;
        private RelaySimpleCommand _RelaySimpleCommand;

        public RelaySimpleCommandTest() 
        {
            _Action = Substitute.For<Action>();
            _RelaySimpleCommand = new RelaySimpleCommand(_Action);
        }

        [Fact]
        public void Execute_Without_Argument_Call_Action()
        {
            _RelaySimpleCommand.Execute();

            _Action.Received(1).Invoke();
        }

        [Fact]
        public void Execute_With_Argument_Call_Action() 
        {
            _RelaySimpleCommand.Execute(null);

            _Action.Received(1).Invoke();
        }

        //[Fact]
        //public void RelayResultCommandShouldCallFunctionGeneric()
        //{
        //    var function = Substitute.For<Func<object,int>>();
        //    var arg = new object();
        //    function.Invoke(arg).Returns(122);
        //    var target = new RelayResultCommand<object,int>(function);
      
        //    var res = target.Execute(arg).Result;

        //    function.Received(1).Invoke(arg);
        //    res.Should().Be(122);
        //}

        //[Fact]
        //public void RelayResultCommandCreate_CreateCommandThatCallFunctionGeneric()
        //{
        //    var function = Substitute.For<Func<object, int>>();
        //    var arg = new object();
        //    function.Invoke(arg).Returns(122);
        //    var target = RelayResultCommand.Create(function);

        //    var res = target.Execute(arg).Result;

        //    function.Received(1).Invoke(arg);
        //    res.Should().Be(122);
        //}

        //[Fact]
        //public void RelayResultCommandShouldHandleExceptionFunctionGeneric()
        //{
        //    var exception = new Exception();
        //    var function = Substitute.For<Func<object, int>>();
        //    var arg = new object();
        //    function.When(f => f.Invoke(arg)).Do(_ => { throw exception; });
        //    var target = new RelayResultCommand<object, int>(function);

        //    var res = target.Execute(arg);

        //    function.Received(1).Invoke(arg);
        //    res.IsFaulted.Should().BeTrue();
        //    res.Exception.Flatten().InnerException.Should().Be(exception);
        //}

        //[Fact]
        //public void RelayResultCommandShouldCallFunctionGenericWithTask()
        //{
        //    var function = Substitute.For<Func<object, Task<int>>>();
        //    var tcs = new TaskCompletionSource<int>();
        //    tcs.SetResult(35);
        //    var arg = new object();
        //    function.Invoke(arg).Returns(tcs.Task);
        //    var target = new RelayResultCommand<object, int>(function);

        //    var res = target.Execute(arg).Result;

        //    function.Received(1).Invoke(arg);
        //    res.Should().Be(35);
        //}

        //[Fact]
        //public void RelayResultCommandShouldCallFunction()
        //{
        //    var function = Substitute.For<Func<int>>();
        //    function.Invoke().Returns(12);
        //    var target = RelayResultCommand.Create(function);
        //    var arg = new object();

        //    var res = target.Execute(arg).Result;

        //    function.Received(1).Invoke();
        //    res.Should().Be(12);
        //}
    }
}
