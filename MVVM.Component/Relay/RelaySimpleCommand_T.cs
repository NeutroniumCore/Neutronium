using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.Component
{
    public class RelaySimpleCommand<T> : ISimpleCommand where T:class
    {
        private Action<T> _Do;

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
