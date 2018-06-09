using System;
using FluentAssertions;
using Neutronium.MVVMComponents.Relay;
using NSubstitute;
using Xunit;

namespace Neutronium.MVVMComponents.Test
{
    public class RelayToogleCommandTest
    {
        private readonly Action _Action;
        private readonly RelayToogleCommand _RelayToogleCommand;

        public RelayToogleCommandTest()
        {
            _Action = Substitute.For<Action>();
            _RelayToogleCommand = new RelayToogleCommand(_Action);
        }

        [Fact]
        public void Execute_without_argument_call_action()
        {
            _RelayToogleCommand.Execute();

            _Action.Received(1).Invoke();
        }

        [Fact]
        public void Execute_with_argument_call_action()
        {
            _RelayToogleCommand.Execute(null);

            _Action.Received(1).Invoke();
        }

        [Fact]
        public void Execute_without_argument_does_not_call_action_when_shouldExecute_is_false()
        {
            _RelayToogleCommand.ShouldExecute = false;
            _RelayToogleCommand.Execute();

            _Action.DidNotReceive().Invoke();
        }

        [Fact]
        public void Execute_with_argument_does_not_call_action_when_shouldExecute_is_false()
        {
            _RelayToogleCommand.ShouldExecute = false;
            _RelayToogleCommand.Execute(null);

            _Action.DidNotReceive().Invoke();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(25)]
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
