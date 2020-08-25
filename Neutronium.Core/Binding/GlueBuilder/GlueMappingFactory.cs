using System;
using System.Collections;
using System.Windows.Input;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.GlueObject.Executable;
using Neutronium.Core.Binding.GlueObject.Mapped;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.Binding.Mapper;
using Neutronium.Core.Binding.SessionManagement;
using Neutronium.Core.Infra.Reflection;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueBuilder
{
    internal sealed class GlueMappingFactory : GlueFactoryBase, IGlueFactory 
    {
        public ICSharpUnrootedObjectManager UnrootedObjectManager { set; private get; }

        private readonly IJavascriptToGlueMapper _JavascriptToGlueMapper;
        private readonly HtmlViewContext _HtmlViewContext;

        public GlueMappingFactory(HtmlViewContext context, ICSharpToJsCache cacher, IJavascriptToGlueMapper converter, ObjectChangesListener onListener) :
            base(cacher, onListener)
        {
            _HtmlViewContext = context;
            _JavascriptToGlueMapper = converter;
        }

        public JsCommand Build(ICommand command)
        {
            return Cache(command, new JsMappableCommand(_HtmlViewContext, _JavascriptToGlueMapper, command));
        }

        public JsCommand<T> Build<T>(ICommand<T> command)
        {
            return Cache(command, new JsMappedCommand<T>(_HtmlViewContext, _JavascriptToGlueMapper, command));
        }

        public JsCommandWithoutParameter Build(ICommandWithoutParameter command)
        {
            return Cache(command, new JsMappedCommandWithoutParameter(_HtmlViewContext, _JavascriptToGlueMapper, command)); ;
        }

        public JsSimpleCommand Build(ISimpleCommand command)
        {
            return Cache(command, new JsMappableSimpleCommand(_HtmlViewContext, _JavascriptToGlueMapper, command));
        }

        public JsResultCommand<TArg, TResult> Build<TArg, TResult>(IResultCommand<TArg, TResult> command)
        {
            return Cache(command, new JsMappableResultCommand<TArg, TResult>(_HtmlViewContext, UnrootedObjectManager, _JavascriptToGlueMapper, command));
        }

        public JsResultCommand<TResult> Build<TResult>(IResultCommand<TResult> command)
        {
            return Cache(command, new JsMappableResultCommand<TResult>(_HtmlViewContext, UnrootedObjectManager, _JavascriptToGlueMapper, command));
        }

        public JsGenericObject Build(object from, IGenericPropertyAcessor typePropertyAccessor)
        {
            return Cache(from, new JsMappableGenericObject(from, typePropertyAccessor));
        }

        public JsSimpleCommand<T> Build<T>(ISimpleCommand<T> command) 
        {
            return Cache(command, new JsMappableSimpleCommand<T>(_HtmlViewContext, _JavascriptToGlueMapper, command));
        }

        public JsArray BuildArray(IEnumerable source, Type basictype)
        {
            return Cache(source, new JsMappableArray(source, basictype));
        }
    }
}
