using System;
using System.Threading.Tasks;
using FluentAssertions;
using Neutronium.MVVMComponents.Relay;
using NSubstitute;
using Xunit;

namespace Neutronium.MVVMComponents.Test.Relay 
{
    public class RelayTaskCommandTest 
    {
        private readonly Func<Task> _Execute;
        private readonly RelayTaskCommand _RelayTaskCommand;

        public RelayTaskCommandTest()
        {
            _Execute = Substitute.For<Func<Task>>();
            _RelayTaskCommand = new RelayTaskCommand(_Execute);
        }

        [Fact]
        public void Execute_without_argument_call_action()
        {
            _RelayTaskCommand.Execute();

            _Execute.Received(1).Invoke();
        }

        [Fact]
        public void Execute_with_argument_call_action()
        {
            _RelayTaskCommand.Execute(null);

            _Execute.Received(1).Invoke();
        }

        [Fact]
        public void Execute_without_argument_set_can_execute_to_false() 
        {
            var tcs = new TaskCompletionSource<object>();
            _Execute.Invoke().Returns(tcs.Task);
            _RelayTaskCommand.Execute();

            _RelayTaskCommand.CanExecute(null).Should().BeFalse();
            tcs.SetResult(null);
        }

        [Fact]
        public void Execute_without_argument_set_can_execute_to_true_after_execution() 
        {
            var tcs = new TaskCompletionSource<object>();
            _Execute.Invoke().Returns(tcs.Task);
            _RelayTaskCommand.Execute();
            tcs.SetResult(null);

            _RelayTaskCommand.CanExecute(null).Should().BeTrue();
        }

        [Fact]
        public void Execute_without_argument_fire_CanExecuteChanged()
        {
            var tcs = new TaskCompletionSource<object>();
            _Execute.Invoke().Returns(tcs.Task);

            using (var monitor = _RelayTaskCommand.Monitor()) 
            {
                _RelayTaskCommand.Execute();
                monitor.Should().Raise("CanExecuteChanged").WithSender(_RelayTaskCommand);
            }

            tcs.SetResult(null);
        }

        [Fact]
        public void Execute_without_argument_do_not_call_execute_when_task_is_not_completed() 
        {
            var tcs = new TaskCompletionSource<object>();
            _Execute.Invoke().Returns(tcs.Task);
            _RelayTaskCommand.Execute();

            _Execute.ClearReceivedCalls();
            _RelayTaskCommand.Execute();
            _Execute.DidNotReceive().Invoke();
            tcs.SetResult(null);         
        }

        [Fact]
        public void Execute_without_argument_call_execute_when_task_is_completed() 
        {
            var tcs = new TaskCompletionSource<object>();
            _Execute.Invoke().Returns(tcs.Task);
            _RelayTaskCommand.Execute();
            tcs.SetResult(null);

            _Execute.ClearReceivedCalls();
            _RelayTaskCommand.Execute();
            _Execute.Received(1).Invoke();        
        }
    }
}
