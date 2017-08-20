using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueObject
{
    internal interface IGlueFactory
    {
        JSCommand Build(ICommand command);

        JsSimpleCommand Build(ISimpleCommand command);

        JsResultCommand Build(IResultCommand command);

        JsGenericObject Build(object from, int childrenCount);

        JSArray BuildArray(List<IJSCSGlue> values, IEnumerable source, Type basictype);
    }
}
