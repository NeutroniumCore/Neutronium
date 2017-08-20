using System.Windows.Input;
using Neutronium.MVVMComponents;
using System.Collections.Generic;
using System.Collections;
using System;

namespace Neutronium.Core.Binding.GlueObject.Mapped
{
    internal class GlueMappingFactory : IGlueFactory
    {
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;
        private readonly HTMLViewContext _HTMLViewContext;

        public GlueMappingFactory(HTMLViewContext context, IJavascriptToCSharpConverter converter)
        {
            _HTMLViewContext = context;
            _JavascriptToCSharpConverter = converter;
        }

        public JSCommand Build(ICommand command)
        {
            return new JSMappableCommand(_HTMLViewContext, _JavascriptToCSharpConverter, command);
        }

        public JsSimpleCommand Build(ISimpleCommand command)
        {
            return new JsMappableSimpleCommand(_HTMLViewContext, _JavascriptToCSharpConverter, command);
        }

        public JsResultCommand Build(IResultCommand command)
        {
            return new JsMappableResultCommand(_HTMLViewContext, _JavascriptToCSharpConverter, command);
        }

        public JsGenericObject Build(object from, int childrenCount)
        {
            return new JsMappableGenericObject(from, childrenCount);
        }

        public JSArray BuildArray(List<IJSCSGlue> values, IEnumerable source, Type basictype)
        {
            return new JSMappableArray(values, source, basictype);
        }
    }
}
