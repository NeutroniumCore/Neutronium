using System;

namespace MVVM.Component
{
    public class RelaySimpleCommand : ISimpleCommand
    {
        private readonly Action _Do;

        public RelaySimpleCommand(Action iDo)
        {
            _Do = iDo;
        }

        public void Execute(object argument)
        {
            _Do();
        }
    }
}
