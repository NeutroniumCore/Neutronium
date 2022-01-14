﻿using System;
using System.Collections;
using System.Windows.Input;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.GlueObject.Basic;
using Neutronium.Core.Binding.GlueObject.Executable;
using Neutronium.Core.Binding.SessionManagement;
using Neutronium.Core.Infra.Reflection;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueBuilder
{
    public interface IGlueFactory
    {
        ICSharpUnrootedObjectManager UnrootedObjectManager { set; }

        JsCommand Build(ICommand command);

        JsCommand<T> Build<T>(ICommand<T> command);

        JsCommandWithoutParameter Build(ICommandWithoutParameter command);

        JsSimpleCommand Build(ISimpleCommand command);

        JsSimpleCommand<T> Build<T>(ISimpleCommand<T> command);

        JsResultCommand<TArg, TResult> Build<TArg, TResult>(IResultCommand<TArg, TResult> command);

        JsResultCommand<TResult> Build<TResult>(IResultCommand<TResult> command);

        JsGenericObject Build(object from, IGenericPropertyAcessor typePropertyAccessor);

        JsArray BuildArray(IEnumerable source, Type basictype);

        JsInt BuildInt(object value);

        JsString BuildString(object value);

        JsBool BuildBool(object value);

        JsEnum BuildEnum(object value);

        JsDouble BuildDouble(object value);

        JsByte BuildByte(object value);

        JsSByte BuildSByte(object value);

        JsUint BuildUint(object value);

        JsDecimal BuildDecimal(object value);

        JsLong BuildLong(object value);

        JsShort BuildShort(object value);

        JsFloat BuildFloat(object value);

        JsUlong BuildUlong(object value);

        JsUshort BuildUshort(object value);

        JsChar BuildChar(object value);

        JsDateTime BuildDateTime(object value);

        event EventHandler<IJsCsGlue> ElementCreated;    
    }
}
