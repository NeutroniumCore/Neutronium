using System;
using Neutronium.Core.Binding.GlueObject.Factory;

namespace Neutronium.Core.Binding.GlueBuilder 
{
    internal class GlueCollectionsBuilder 
    {
        private readonly IGlueFactory _Factory;
        private readonly CSharpToJavascriptConverter _Converter;

        public GlueCollectionsBuilder(IGlueFactory factory, CSharpToJavascriptConverter converter) 
        {
            _Factory = factory;
            _Converter = converter;
        }

        public ICsToGlueConverter GetCollection(Type collectionType) 
        {
            return new GlueCollectionBuilder(_Factory, _Converter, collectionType);
        }

        public ICsToGlueConverter GetList(Type collectionType)
        {
            return new GlueListBuilder(_Factory, _Converter, collectionType);
        }

        public ICsToGlueConverter GetEnumerable(Type collectionType)
        {
            return new GlueEnumerableBuilder(_Factory, _Converter, collectionType);
        }
    }
}
