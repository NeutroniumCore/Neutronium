using System;
using System.Collections;
using System.Windows.Input;
using Neutronium.Core.Infra.Reflection;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueObject.Factory
{
    internal interface IGlueFactory
    {
        JsCommand Build(ICommand command);

        JsSimpleCommand Build(ISimpleCommand command);

        JsResultCommand Build(IResultCommand command);

        JsGenericObject Build(object from, TypePropertyAccessor typePropertyAccessor);

        JsArray BuildArray(IEnumerable source, Type basictype);

        JsBasicObject BuildBasic(object basic);

        JsInt BuildInt(int value);

        event EventHandler<IJsCsGlue> ElementCreated;
    }
}
