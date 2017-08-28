using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueObject.Factory
{
    internal sealed class GlueFactory : GlueFactoryBase, IGlueFactory 
    {
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;
        private readonly HtmlViewContext _HtmlViewContext;

        public GlueFactory(HtmlViewContext context, ICSharpToJsCache cacher, IJavascriptToCSharpConverter converter)
            :base(cacher)
        {
            _HtmlViewContext = context;
            _JavascriptToCSharpConverter = converter;
        }

        public JsCommand Build(ICommand command)
        {
            return Cache(command, new JsCommand(_HtmlViewContext, _JavascriptToCSharpConverter, command));
        }

        public JsSimpleCommand Build(ISimpleCommand command)
        {
            return Cache(command, new JsSimpleCommand(_HtmlViewContext, _JavascriptToCSharpConverter, command));
        }

        public JsResultCommand Build(IResultCommand command)
        {
            return Cache(command, new JsResultCommand(_HtmlViewContext, _JavascriptToCSharpConverter, command));
        }

        public JsGenericObject Build(object from, int childrenCount)
        {
            return Cache(from, new JsGenericObject(from, childrenCount));
        }

        public JsArray BuildArray(List<IJsCsGlue> enumerable, IEnumerable source, Type basictype)
        {
            return Cache(source, new JsArray(enumerable, source, basictype));
        }
    }
}
