using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.CEFGlue.Binding.HTMLBinding.V8JavascriptObject
{
    public interface IJavascriptObjectFactory
    {
        IJavascriptObject CreateNull();

        IJavascriptObject CreateUndefined();

        IJavascriptObject CreateInt(int ivalue);

        IJavascriptObject CreateDouble(double ivalue);

        IJavascriptObject CreateString(string ivalue);

        IJavascriptObject CreateObject(int Id, string ivalue);
    }
}
