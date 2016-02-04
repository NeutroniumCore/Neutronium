using System;

namespace MVVM.Component
{
    public class RelaySimpleCommand<T> : ISimpleCommand where T:class
    {
        private readonly Action<T> _Do;

        public RelaySimpleCommand(Action<T> iDo)
        {
            _Do = iDo;
        }

        public void Execute(object argument)
        {
            _Do(argument as T);
        }
    }
}
