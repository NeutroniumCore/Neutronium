using System;
using System.Collections;
using System.Windows.Input;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.GlueObject.Executable;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.Infra.Reflection;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueBuilder
{
    internal sealed class GlueFactory : GlueFactoryBase, IGlueFactory 
    {
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;
        private readonly HtmlViewContext _HtmlViewContext;

        public GlueFactory(HtmlViewContext context, ICSharpToJsCache cacher, IJavascriptToCSharpConverter converter, ObjectChangesListener onListener)
            :base(cacher, onListener)
        {
            _HtmlViewContext = context;
            _JavascriptToCSharpConverter = converter;
        }

        public JsCommand Build(ICommand command)
        {
            return Cache(command, new JsCommand(_HtmlViewContext, _JavascriptToCSharpConverter, command));
        }

        public JsCommand<T> Build<T>(ICommand<T> command)
        {
            return Cache(command, new JsCommand<T>(_HtmlViewContext, _JavascriptToCSharpConverter, command));
        }

        public JsCommandWithoutParameter Build(ICommandWithoutParameter command)
        {
            return Cache(command, new JsCommandWithoutParameter(_HtmlViewContext, _JavascriptToCSharpConverter, command)); ;
        }

        public JsSimpleCommand Build(ISimpleCommand command)
        {
            return Cache(command, new JsSimpleCommand(_HtmlViewContext, _JavascriptToCSharpConverter, command));
        }

        public JsSimpleCommand<T> Build<T>(ISimpleCommand<T> command) 
        {
            return Cache(command, new JsSimpleCommand<T>(_HtmlViewContext, _JavascriptToCSharpConverter, command));
        }

        public JsResultCommand<TArg,TResult> Build<TArg, TResult>(IResultCommand<TArg, TResult> command)
        {
            return Cache(command, new JsResultCommand<TArg, TResult>(_HtmlViewContext, _JavascriptToCSharpConverter, command));
        }

        public JsResultCommand<TResult> Build<TResult>(IResultCommand<TResult> command)
        {
            return Cache(command, new JsResultCommand<TResult>(_HtmlViewContext, _JavascriptToCSharpConverter, command));
        }

        public JsGenericObject Build(object from, IGenericPropertyAcessor typePropertyAccessor)
        {
            return Cache(from, new JsGenericObject(from, typePropertyAccessor));
        }

        public JsArray BuildArray(IEnumerable source, Type basictype)
        {
            return Cache(source, new JsArray(source, basictype));
        }
    }
}
