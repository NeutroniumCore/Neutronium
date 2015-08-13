using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.CEFGlue.Binding.HTMLBinding.V8JavascriptObject
{
    public interface IJavascriptObject
    {
        bool IsUndefined { get;  }

        bool IsNull { get; }
    }
}
