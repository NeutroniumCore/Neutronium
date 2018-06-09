using System;
using FluentAssertions;
using Neutronium.MVVMComponents.Relay;
using NSubstitute;
using Xunit;

namespace Neutronium.MVVMComponents.Test.Relay
{
    public class RelayToogleCommandGenericTest
    {
        private readonly Action<string> _Action;
        private readonly RelayToogleCommand<string> _RelayToogleCommand;

        public RelayToogleCommandGenericTest()
        {
            _Action = Substitute.For<Action<string>>();
            _RelayToogleCommand = new RelayToogleCommand<string>(_Action);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("parameter")]
        public void Execute_without_generic_argument_call_action(string parameter)
        {
            _RelayToogleCommand.Execute(parameter);
            _Action.Received(1).Invoke(parameter);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("parameter")]
        public void Execute_with_object_argument_call_action(object parameter)
        {
            _RelayToogleCommand.Execute(parameter);

            _Action.Received(1).Invoke((string)parameter);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(false)]
        [InlineData(22.4)]
        public void Execute_with_object_argument_does_not_call_action_with_diferent_types(object parameter)
        {
            _RelayToogleCommand.Execute(parameter);

            _Action.DidNotReceive().Invoke(Arg.Any<string>());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("parameter")]
        public void Execute_without_argument_does_not_call_action_when_shouldExecute_is_false(string parameter)
        {
            _RelayToogleCommand.ShouldExecute = false;
            _RelayToogleCommand.Execute(parameter);

            _Action.DidNotReceive().Invoke(Arg.Any<string>());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("parameter")]
        public void Execute_with_argument_does_not_call_action_when_shouldExecute_is_false(object parameter)
        {
            _RelayToogleCommand.ShouldExecute = false;
            _RelayToogleCommand.Execute(parameter);

            _Action.DidNotReceive().Invoke(Arg.Any<string>());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("parameter")]
        public void CanExecute_returns_true_by_default(object argument)
        {
            var canExecute = _RelayToogleCommand.CanExecute(argument);

            canExecute.Should().BeTrue();
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData(25, true)]
        [InlineData(null, false)]
        [InlineData(25, false)]
        public void CanExecute_returns_ShouldExecute_value(object argument, bool shouldExecute)
        {
            _RelayToogleCommand.ShouldExecute = shouldExecute;
            var canExecute = _RelayToogleCommand.CanExecute(argument);

            canExecute.Should().Be(shouldExecute);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanExecuteChanged_is_raised_when_shouldExecute_changes_value(bool shouldExecute)
        {
            _RelayToogleCommand.ShouldExecute = !shouldExecute;

            using (var monitor = _RelayToogleCommand.Monitor()) 
            {
                _RelayToogleCommand.ShouldExecute = shouldExecute;
                monitor.Should().Raise("CanExecuteChanged").WithSender(_RelayToogleCommand);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanExecuteChanged_is_not_raised_when_shouldExecute_does_not_changes_value(bool shouldExecute)
        {
            _RelayToogleCommand.ShouldExecute = shouldExecute;

            using (var monitor = _RelayToogleCommand.Monitor()) 
            {
                _RelayToogleCommand.ShouldExecute = shouldExecute;
                monitor.Should().NotRaise("CanExecuteChanged");
            }
        }
    }
}
