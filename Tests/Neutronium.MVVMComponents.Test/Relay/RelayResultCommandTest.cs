using System;
using System.Threading.Tasks;
using FluentAssertions;
using Neutronium.MVVMComponents.Relay;
using NSubstitute;
using Xunit;

namespace Neutronium.MVVMComponents.Test.Relay 
{
    public class RelayResultCommandTest 
    {
        [Fact]
        public void Execute_Call_Generic_Function() 
        {
            var function = Substitute.For<Func<object, int>>();
            var arg = new object();
            function.Invoke(arg).Returns(122);
            var target = new RelayResultCommand<object, int>(function);

            var res = target.Execute(arg).Result;

            function.Received(1).Invoke(arg);
            res.Should().Be(122);
        }

        [Fact]
        public void Create_Create_Command_That_Calls_Generic_Function() 
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
        public void Execute_Handle_Exception() 
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
        public void Execute_Call_Generic_Function_With_Task()
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
        public void Execute_Call_Function() 
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
