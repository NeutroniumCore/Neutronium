using System;
using Xunit;
using FluentAssertions;
using System.Threading.Tasks;
using MVVM.Component.Relay;
using NSubstitute;

namespace MVVM.Component.Test
{
    public class RelaySimpleCommandTest
    {
        [Fact]
        public void RelaySimpleCommandShouldCallActionGeneric()
        {
            var action = Substitute.For<Action<object>>();
            var target = new RelaySimpleCommand<object>(action);
            var arg = new object();

            target.Execute(arg);

            action.Received(1).Invoke(arg);
        }

        [Fact]
        public void RelaySimpleCommandShouldCallAction()
        {
            var action = Substitute.For<Action>();
            var target = new RelaySimpleCommand(action);
            var arg = new object();

            target.Execute(arg);

            action.Received(1).Invoke();
        }

        [Fact]
        public void RelayResultCommandShouldCallFunctionGeneric()
        {
            var function = Substitute.For<Func<object,int>>();
            var arg = new object();
            function.Invoke(arg).Returns(122);
            var target = new RelayResultCommand<object,int>(function);
      
            var res = target.Execute(arg).Result;

            function.Received(1).Invoke(arg);
            res.Should().Be(122);
        }

        [Fact]
        public void RelayResultCommandCreate_CreateCommandThatCallFunctionGeneric()
        {
            var function = Substitute.For<Func<object, int>>();
            var arg = new object();
            function.Invoke(arg).Returns(122);
            var target = RelayResultCommand.Create(function);

            var res = target.Execute(arg).Result;

            function.Received(1).Invoke(arg);
            res.Should().Be(122);
        }

        [Fact]
        public void RelayResultCommandShouldHandleExceptionFunctionGeneric()
        {
            var exception = new Exception();
            var function = Substitute.For<Func<object, int>>();
            var arg = new object();
            function.When(f => f.Invoke(arg)).Do(_ => { throw exception; });
            var target = new RelayResultCommand<object, int>(function);

            var res = target.Execute(arg);

            function.Received(1).Invoke(arg);
            res.IsFaulted.Should().BeTrue();
            res.Exception.Flatten().InnerException.Should().Be(exception);
        }

        [Fact]
        public void RelayResultCommandShouldCallFunctionGenericWithTask()
        {
            var function = Substitute.For<Func<object, Task<int>>>();
            var tcs = new TaskCompletionSource<int>();
            tcs.SetResult(35);
            var arg = new object();
            function.Invoke(arg).Returns(tcs.Task);
            var target = new RelayResultCommand<object, int>(function);

            var res = target.Execute(arg).Result;

            function.Received(1).Invoke(arg);
            res.Should().Be(35);
        }

        [Fact]
        public void RelayResultCommandShouldCallFunction()
        {
            var function = Substitute.For<Func<int>>();
            function.Invoke().Returns(12);
            var target = RelayResultCommand.Create(function);
            var arg = new object();

            var res = target.Execute(arg).Result;

            function.Received(1).Invoke();
            res.Should().Be(12);
        }
    }
}
