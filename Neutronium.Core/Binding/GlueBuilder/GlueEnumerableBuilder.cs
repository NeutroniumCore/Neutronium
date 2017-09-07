using System;
using System.Collections;
using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.GlueObject.Factory;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.Core.Binding.GlueBuilder 
{
    internal sealed class GlueEnumerableBuilder : GlueAllCollectionBuilder, ICsToGlueConverter
    {
        internal GlueEnumerableBuilder(IGlueFactory factory, IWebBrowserWindow context, CSharpToJavascriptConverter converter, Type collectionType):
                        base(factory, context, converter, collectionType)
        {
        }

        public IJsCsGlue Convert(object @object) 
        {
            var enumerable = (IEnumerable)@object;
            var arrayResult = Build(enumerable);
            arrayResult.SetChildren(Convert(enumerable));
            return arrayResult;
        }

        private List<IJsCsGlue> Convert(IEnumerable collection) 
        {
            var list = new List<IJsCsGlue>();
            return AppendToList(list, collection);
        }
    }
}
