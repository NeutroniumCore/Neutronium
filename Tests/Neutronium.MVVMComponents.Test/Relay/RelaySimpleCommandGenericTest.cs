using System;
using FluentAssertions;
using Neutronium.MVVMComponents.Relay;
using NSubstitute;
using Xunit;

namespace Neutronium.MVVMComponents.Test.Relay
{
    public class RelaySimpleCommandGenericTest
    {
        private readonly Action<string> _Action;
        private readonly RelaySimpleCommand<string> _RelaySimpleCommand;

        public RelaySimpleCommandGenericTest() 
        {
            _Action = Substitute.For<Action<string>>();
            _RelaySimpleCommand = new RelaySimpleCommand<string>(_Action);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("parameter")]
        public void Execute_without_generic_argument_call_action(string parameter) 
        {
            _RelaySimpleCommand.Execute(parameter);
            _Action.Received(1).Invoke(parameter);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("parameter")]
        public void Execute_with_object_argument_call_action(object parameter) 
        {
            _RelaySimpleCommand.Execute(parameter);

            _Action.Received(1).Invoke((string)parameter);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(25.8)]
        [InlineData(true)]
        public void Execute_with_object_argument_does_not_call_action_if_type_mismatch(object parameter)
        {
            _RelaySimpleCommand.Execute(parameter);

            _Action.DidNotReceive().Invoke(Arg.Any<string>());
        }

        [Fact]
        public void Execute_with_object_argument_does_not_call_action_if_type_mismatch_null_for_int()
        {
            var action = Substitute.For<Action<int>>();
            var relaySimpleCommand = new RelaySimpleCommand<int>(action);

            relaySimpleCommand.Execute(null);
            action.DidNotReceive().Invoke(Arg.Any<int>());
        }

        [Theory]
        [InlineData(null)]
        [InlineData(25)]
        public void CanExecute_returns_true(object argument) 
        {
            var canExecute = _RelaySimpleCommand.CanExecute(argument);

            canExecute.Should().BeTrue();
        }
    }
}
