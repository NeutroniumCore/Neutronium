using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;
using System.Collections.Generic;

namespace VueUiFramework
{
    internal class JavascriptTreeVisiter
    {
        private readonly IJavascriptObject _Root;
        private readonly IJavascriptObjectMapper _Mapper;
        private readonly HashSet<IJavascriptObject> _Mapped = new HashSet<IJavascriptObject>();

        internal JavascriptTreeVisiter(IJavascriptObject root, IJavascriptObjectMapper mapper)
        {
            _Root = root;
            _Mapper = mapper;
        }

        internal void Visit()
        {
            _Mapper.MapFirst(_Root);
            MapChild(_Root, null);
            _Mapper.EndMapping(_Root);
        }

        private void MapChild(IJavascriptObject parent, string attributeName)
        {
            if (!_Mapped.Add(parent))
                return;

            if (parent.IsObject)
                MapChildObject(parent);
            else if (parent.IsArray)
                MapChildArray(parent, attributeName);
        }

        private void MapChildObject(IJavascriptObject parent)
        {
            foreach(var childkey in parent.GetAttributeKeys())
            {
                var child = parent.GetValue(childkey);
                _Mapper.Map(parent, childkey, child);
                MapChild(child, childkey);
            }
        }

        private void MapChildArray(IJavascriptObject parent, string attributeName)
        {
            int i = 0;
            foreach (var child in parent.GetArrayElements())
            {
                _Mapper.MapCollection(parent, attributeName, i++, child);
                MapChild(child, null);
            }
        }
    }
}
