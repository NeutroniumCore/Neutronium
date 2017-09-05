using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueObject.Factory
{
    internal interface IGlueFactory
    {
        JsCommand Build(ICommand command);

        JsSimpleCommand Build(ISimpleCommand command);

        JsResultCommand Build(IResultCommand command);

        JsGenericObject Build(object from);

        JsArray BuildArray(IEnumerable source, Type basictype);

        JsBasicObject BuildBasic(object basic);

        event EventHandler<IJsCsGlue> ElementCreated;
    }
}
