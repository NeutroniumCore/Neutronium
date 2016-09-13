using System;

namespace Neutronium.MVVMComponents.Relay
{
    public class RelaySimpleCommand : ISimpleCommand
    {
        private readonly Action _Do;

        public RelaySimpleCommand(Action doAction)
        {
            _Do = doAction;
        }

        public void Execute(object argument)
        {
            _Do();
        }
    }
}
