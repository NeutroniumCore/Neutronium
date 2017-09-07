using System;
using Neutronium.Core.Binding.GlueObject.Factory;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.Core.Binding.GlueBuilder 
{
    internal class GlueCollectionsBuilder 
    {
        private readonly IGlueFactory _Factory;
        private readonly CSharpToJavascriptConverter _Converter;
        private readonly IWebBrowserWindow _WebBrowserWindow;

        public GlueCollectionsBuilder(IGlueFactory factory, IWebBrowserWindow context, CSharpToJavascriptConverter converter) 
        {
            _Factory = factory;
            _Converter = converter;
            _WebBrowserWindow = context;
        }

        public ICsToGlueConverter GetCollection(Type collectionType) 
        {
            return new GlueCollectionBuilder(_Factory, _WebBrowserWindow, _Converter, collectionType);
        }

        public ICsToGlueConverter GetList(Type collectionType)
        {
            return new GlueListBuilder(_Factory, _WebBrowserWindow, _Converter, collectionType);
        }

        public ICsToGlueConverter GetEnumerable(Type collectionType)
        {
            return new GlueEnumerableBuilder(_Factory, _WebBrowserWindow, _Converter, collectionType);
        }
    }
}
