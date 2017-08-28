using System.Windows.Input;
using Neutronium.MVVMComponents;
using System.Collections.Generic;
using System.Collections;
using System;

namespace Neutronium.Core.Binding.GlueObject.Mapped
{
    internal sealed class GlueMappingFactory : IGlueFactory
    {
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;
        private readonly ICSharpToJsCache _Cacher;
        private readonly HtmlViewContext _HtmlViewContext;

        public GlueMappingFactory(HtmlViewContext context, ICSharpToJsCache cacher, IJavascriptToCSharpConverter converter)
        {
            _HtmlViewContext = context;
            _JavascriptToCSharpConverter = converter;
            _Cacher = cacher;
        }

        public JsCommand Build(ICommand command)
        {
            return new JsMappableCommand(_HtmlViewContext, _JavascriptToCSharpConverter, command);
        }

        public JsSimpleCommand Build(ISimpleCommand command)
        {
            return new JsMappableSimpleCommand(_HtmlViewContext, _JavascriptToCSharpConverter, command);
        }

        public JsResultCommand Build(IResultCommand command)
        {
            return new JsMappableResultCommand(_HtmlViewContext, _JavascriptToCSharpConverter, command);
        }

        public JsGenericObject Build(object from, int childrenCount)
        {
            return new JsMappableGenericObject(from, childrenCount);
        }

        public JsArray BuildArray(List<IJsCsGlue> values, IEnumerable source, Type basictype)
        {
            return new JsMappableArray(values, source, basictype);
        }
    }
}
