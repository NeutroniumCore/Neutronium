using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueObject
{
    internal sealed class GlueFactory : IGlueFactory
    {
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;
        private readonly HTMLViewContext _HTMLViewContext;

        public GlueFactory(HTMLViewContext context, IJavascriptToCSharpConverter converter)
        {
            _HTMLViewContext = context;
            _JavascriptToCSharpConverter = converter;
        }

        public JSCommand Build(ICommand command)
        {
            return new JSCommand(_HTMLViewContext, _JavascriptToCSharpConverter, command);
        }

        public JsSimpleCommand Build(ISimpleCommand command)
        {
            return new JsSimpleCommand(_HTMLViewContext, _JavascriptToCSharpConverter, command);
        }

        public JsResultCommand Build(IResultCommand command)
        {
            return new JsResultCommand(_HTMLViewContext, _JavascriptToCSharpConverter, command);
        }

        public JsGenericObject Build(object from, int childrenCount)
        {
            return new JsGenericObject(from, childrenCount);
        }

        public JsArray BuildArray(List<IJsCsGlue> enumerable, IEnumerable source, Type basictype)
        {
            return new JsArray(enumerable, source, basictype);
        }
    }
}
