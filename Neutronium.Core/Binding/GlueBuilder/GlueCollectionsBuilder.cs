using System;
using System.Collections;
using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.GlueObject.Factory;
using Neutronium.Core.Infra;

namespace Neutronium.Core.Binding.GlueBuilder 
{
    internal sealed class GlueCollectionsBuilder
    {
        private readonly Type _BasicType;
        private readonly CSharpToJavascriptConverter _Converter;

        internal GlueCollectionsBuilder(CSharpToJavascriptConverter converter, Type collectionType)
        {
            _Converter = converter;

            var elementType = collectionType.GetEnumerableBase();
            elementType = elementType?.GetUnderlyingType();
            _BasicType = converter.IsBasicType(elementType) ? elementType : null;
        }

        internal IJsCsGlue ConvertList(IGlueFactory factory, object @object)
        {
            var collection = (IList)@object;
            var arrayResult = Build(factory, collection);
            arrayResult.SetChildren(Convert(collection));
            return arrayResult;
        }

        internal IJsCsGlue ConvertCollection(IGlueFactory factory, object @object) 
        {
            var collection = (ICollection)@object;
            var arrayResult = Build(factory, collection);
            arrayResult.SetChildren(Convert(collection));
            return arrayResult;
        }

        internal IJsCsGlue ConvertEnumerable(IGlueFactory factory, object @object)
        {
            var enumerable = (IEnumerable)@object;
            var arrayResult = Build(factory, enumerable);
            arrayResult.SetChildren(Convert(enumerable));
            return arrayResult;
        }

        private List<IJsCsGlue> Convert(IList collection)
        {
            var list = new List<IJsCsGlue>(collection.Count);
            return AppendToList(list, collection);
        }

        private List<IJsCsGlue> Convert(ICollection collection) 
        {
            var list = new List<IJsCsGlue>(collection.Count);
            return AppendToList(list, collection);
        }

        private List<IJsCsGlue> Convert(IEnumerable collection)
        {
            var list = new List<IJsCsGlue>();
            return AppendToList(list, collection);
        }

        private JsArray Build(IGlueFactory factory, IEnumerable enumerable)
        {
            return factory.BuildArray(enumerable, _BasicType);
        }

        private List<IJsCsGlue> AppendToList(List<IJsCsGlue> childrenList, IEnumerable enumerable)
        {
            foreach (var @object in enumerable)
            {
                childrenList.Add(_Converter.Map(@object).AddRef());
            }
            return childrenList;
        }

        private List<IJsCsGlue> AppendToList(List<IJsCsGlue> childrenList, IList list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                childrenList.Add(_Converter.Map(list[i]).AddRef());
            }
            return childrenList;
        }
    }
}
