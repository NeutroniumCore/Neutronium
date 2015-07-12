using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.Component
{
    public interface ISimpleCommand
    {
        void Execute(object argument);
    }
}
