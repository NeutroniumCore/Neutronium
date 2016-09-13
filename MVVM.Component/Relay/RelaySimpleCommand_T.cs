using System;

namespace Neutronium.MVVMComponents.Relay
{
    public class RelaySimpleCommand<T> : ISimpleCommand where T:class
    {
        private readonly Action<T> _Do;

        public RelaySimpleCommand(Action<T> doAction)
        {
            _Do = doAction;
        }

        public void Execute(object argument)
        {
            _Do(argument as T);
        }
    }
}
