using System;

namespace Neutronium.MVVMComponents.Relay
{
    /// <summary>
    /// ISimpleCommand implementation based on action with no argument
    /// <seealso cref="ISimpleCommand"/>
    /// </summary>
    public class RelaySimpleCommand : ISimpleCommand
    {
        private readonly Action _Do;

        public RelaySimpleCommand(Action doAction)
        {
            _Do = doAction;
        }

        public void Execute()
        {
            _Do();
        }
    }
}
