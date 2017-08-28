using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueObject
{
    internal interface IGlueFactory
    {
        JsCommand Build(ICommand command);

        JsSimpleCommand Build(ISimpleCommand command);

        JsResultCommand Build(IResultCommand command);

        JsGenericObject Build(object from, int childrenCount);

        JsArray BuildArray(List<IJsCsGlue> values, IEnumerable source, Type basictype);

        JsBasicObject BuildBasic(object basic);

        event EventHandler<IJsCsGlue> ElementCreated;
    }
}
