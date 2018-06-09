using System;
using System.Threading.Tasks;
using FluentAssertions;
using Neutronium.MVVMComponents.Relay;
using NSubstitute;
using Xunit;

namespace Neutronium.MVVMComponents.Test.Relay 
{
    public class RelayTaskCommandGenericTest 
    {
        private readonly Func<string, Task> _Execute;
        private readonly RelayTaskCommand<string> _RelayTaskCommand;

        public RelayTaskCommandGenericTest()
        {
            _Execute = Substitute.For<Func<string, Task>>();
            _RelayTaskCommand = new RelayTaskCommand<string>(_Execute);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("parameter")]
        public void Execute_with_argument_generic_call_action(string parameter)
        {
            _RelayTaskCommand.Execute(parameter);

            _Execute.Received(1).Invoke(parameter);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("parameter")]
        public void Execute_with_argument_call_action(object parameter)
        {
            _RelayTaskCommand.Execute(parameter);

            _Execute.Received(1).Invoke((string)parameter);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(25.8)]
        [InlineData(true)]
        public void Execute_with_object_argument_does_not_call_action_if_type_mismatch(object parameter) 
        {
            _RelayTaskCommand.Execute(parameter);

            _Execute.DidNotReceive().Invoke(Arg.Any<string>());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("parameter")]
        public void Execute_with_generic_argument_set_can_execute_to_false(string parameter) 
        {
            var tcs = new TaskCompletionSource<object>();
            _Execute.Invoke(parameter).Returns(tcs.Task);
            _RelayTaskCommand.Execute(parameter);

            _RelayTaskCommand.CanExecute(null).Should().BeFalse();
            tcs.SetResult(null);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("parameter")]
        public void Execute_with_generic_argument_set_can_execute_to_true_after_execution(string parameter) 
        {
            var tcs = new TaskCompletionSource<object>();
            _Execute.Invoke(parameter).Returns(tcs.Task);
            _RelayTaskCommand.Execute(parameter);
            tcs.SetResult(null);

            _RelayTaskCommand.CanExecute(null).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("parameter")]
        public void Execute_with_generic_argument_fire_CanExecuteChanged(string parameter) 
        {
            var tcs = new TaskCompletionSource<object>();
            _Execute.Invoke(parameter).Returns(tcs.Task);

            using (var monitor = _RelayTaskCommand.Monitor()) 
            {
                _RelayTaskCommand.Execute(parameter);
                monitor.Should().Raise("CanExecuteChanged").WithSender(_RelayTaskCommand);
            }

            tcs.SetResult(null);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("parameter")]
        public void Execute_with_generic_argument_do_not_call_execute_when_task_is_not_completed(string parameter) 
        {
            var tcs = new TaskCompletionSource<object>();
            _Execute.Invoke(parameter).Returns(tcs.Task);
            _RelayTaskCommand.Execute(parameter);

            _Execute.ClearReceivedCalls();
            _RelayTaskCommand.Execute(parameter);
            _Execute.DidNotReceive().Invoke(Arg.Any<string>());
            tcs.SetResult(null);         
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("parameter")]
        public void Execute_with_generic_argument_call_execute_when_task_is_completed(string parameter) 
        {
            var tcs = new TaskCompletionSource<object>();
            _Execute.Invoke(parameter).Returns(tcs.Task);
            _RelayTaskCommand.Execute(parameter);
            tcs.SetResult(null);

            _Execute.ClearReceivedCalls();
            _RelayTaskCommand.Execute(parameter);
            _Execute.Received(1).Invoke(parameter);        
        }
    }
}
