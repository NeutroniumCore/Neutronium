using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;
using System.Collections.Generic;
using System.Linq;

namespace VueUiFramework
{
    internal class JavascriptTreeVisiter
    {
        private readonly IWebView _WebView;
        private readonly IJavascriptObject _Root;
        private readonly IJavascriptObjectMapper _Mapper;
        private readonly HashSet<IJavascriptObject> _Mapped = new HashSet<IJavascriptObject>();

        internal JavascriptTreeVisiter(IWebView webView, IJavascriptObject root, IJavascriptObjectMapper mapper)
        {
            _WebView = webView;
            _Root = root;
            _Mapper = mapper;
        }

        internal void Visit()
        {
            _Mapper.MapFirst(_Root);
            MapChild(_Root);
            _Mapper.EndMapping(_Root);
        }

        private void MapChild(IJavascriptObject parent)
        {
            if (!_Mapped.Add(parent))
                return;

            if (parent.IsArray)
                MapChildArray(parent);
            else if (parent.IsObject)
                MapChildObject(parent);
        }

        private void MapChildObject(IJavascriptObject parent)
        {
            foreach(var childkey in GetObjectKeys(parent))
            {
                var child = parent.GetValue(childkey);

                if (!IsMapable(child))
                    continue;

                _Mapper.Map(parent, childkey, child);
                MapChild(child);
            }
        }

        private bool IsMapable(IJavascriptObject jsObject)
        {
            if (jsObject.IsArray)
                return true;

            return jsObject.IsObject && !IsEnum(jsObject);
        }


        private IEnumerable<string> GetObjectKeys(IJavascriptObject jobject)
        {
            //return jobject.GetAttributeKeys();
            var extractor = GetExtractor();
            var res = extractor.Invoke("getkeys", _WebView, jobject);
            return res.GetArrayElements().Select(el => el.GetStringValue());
        }


        private bool IsEnum(IJavascriptObject jobject)
        {          
            var extractor = GetExtractor();
            var res = extractor.Invoke("isEnum", _WebView, jobject);
            return res.GetBoolValue();
        }

        private IJavascriptObject _Extractor;
        private IJavascriptObject GetExtractor()
        {
            if (_Extractor != null)
                return _Extractor;

            _WebView.Eval("(function(){return { getkeys: function (ob){ var list = []; for (var property in ob) {  if (ob.hasOwnProperty(property)) { list.push(property) } } return list; }, isEnum :function(arg){return (arg instanceof Enum);} }})()", out _Extractor);
            return _Extractor;
        }

        private void MapChildArray(IJavascriptObject parent)
        {
            int i = 0;
            foreach (var child in parent.GetArrayElements())
            {
                if (!IsMapable(child))
                    continue;

                _Mapper.MapCollection(parent, null, i++, child);
                MapChild(child);
            }
        }
    }
}
