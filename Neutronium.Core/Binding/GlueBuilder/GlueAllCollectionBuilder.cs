using System;
using System.Collections;
using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.GlueObject.Factory;
using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.Core.Binding.GlueBuilder 
{
    internal class GlueAllCollectionBuilder 
    {
        private readonly Type _BasicType;
        private readonly IGlueFactory _GlueFactory;
        private readonly CSharpToJavascriptConverter _Converter;

        protected GlueAllCollectionBuilder(IGlueFactory factory, IWebBrowserWindow context, CSharpToJavascriptConverter converter, Type collectionType)
        {
            _GlueFactory = factory;
            _Converter = converter;

            var elementType = collectionType.GetEnumerableBase();
            _BasicType = elementType?.GetUnderlyingType();
            var basictype = context.IsTypeBasic(elementType) ? elementType : null;
        }

        protected JsArray Build(IEnumerable enumerable) 
        {
            return _GlueFactory.BuildArray(enumerable, _BasicType);
        }

        protected List<IJsCsGlue> AppendToList(List<IJsCsGlue> childrenList, IEnumerable enumerable) 
        {
            foreach (var @object in enumerable) 
            {
                childrenList.Add(_Converter.Map(@object).AddRef());
            }
            return childrenList;
        }

        protected List<IJsCsGlue> AppendToList(List<IJsCsGlue> childrenList, IList list) 
        {
            for(var i=0; i< list.Count; i++) 
            {
                childrenList.Add(_Converter.Map(list[i]).AddRef());
            }
            return childrenList;
        }
    }
}
