using System.Windows.Input;
using Neutronium.MVVMComponents;
using System.Collections.Generic;
using System.Collections;
using System;
using Neutronium.Core.Binding.GlueObject.Mapped;
using Neutronium.Core.Binding.Listeners;

namespace Neutronium.Core.Binding.GlueObject.Factory
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

        public JsGenericObject Build(object from, int childrenCount)
        {
            return Cache(from, new JsMappableGenericObject(from, childrenCount));
        }

        public JsArray BuildArray(List<IJsCsGlue> values, IEnumerable source, Type basictype)
        {
            return Cache(source, new JsMappableArray(values, source, basictype));
        }
    }
}
