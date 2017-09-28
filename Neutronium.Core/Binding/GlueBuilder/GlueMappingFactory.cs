using System;
using System.Collections;
using System.Windows.Input;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.GlueObject.Mapped;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.Infra.Reflection;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueBuilder
{
    internal sealed class GlueMappingFactory : GlueFactoryBase, IGlueFactory 
    {
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;
        private readonly HtmlViewContext _HtmlViewContext;

        public GlueMappingFactory(HtmlViewContext context, ICSharpToJsCache cacher, IJavascriptToCSharpConverter converter, ObjectChangesListener onListener) :
            base(cacher, onListener)
        {
            _HtmlViewContext = context;
            _JavascriptToCSharpConverter = converter;
        }

        public JsCommand Build(ICommand command)
        {
            return Cache(command, new JsMappableCommand(_HtmlViewContext, _JavascriptToCSharpConverter, command));
        }

        public JsSimpleCommand Build(ISimpleCommand command)
        {
            return Cache(command, new JsMappableSimpleCommand(_HtmlViewContext, _JavascriptToCSharpConverter, command));
        }

        public JsResultCommand Build(IResultCommand command)
        {
            return Cache(command, new JsMappableResultCommand(_HtmlViewContext, _JavascriptToCSharpConverter, command));
        }

        public JsGenericObject Build(object from, IGenericPropertyAcessor typePropertyAccessor)
        {
            return Cache(from, new JsMappableGenericObject(from, typePropertyAccessor));
        }

        public JsArray BuildArray(IEnumerable source, Type basictype)
        {
            return Cache(source, new JsMappableArray(source, basictype));
        }
    }
}
