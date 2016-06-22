using MVVM.HTML.Core.JavascriptUIFramework;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace VueUiFramework
{
    internal class VueJavascriptViewModelUpdater : IJavascriptViewModelUpdater
    {
        private IWebView _WebView;

        public VueJavascriptViewModelUpdater(IWebView webView)
        {
            _WebView = webView;
        }

        public void ClearAllCollection(IJavascriptObject array)
        {
        }

        public void Dispose()
        {
        }

        public void MoveCollectionItem(IJavascriptObject array, IJavascriptObject item, int oldIndex, int newIndex)
        {
        }

        public void SpliceCollection(IJavascriptObject array, int index, int number)
        {
        }

        public void SpliceCollection(IJavascriptObject array, int index, int number, IJavascriptObject item)
        {
        }

        public void UpdateProperty(IJavascriptObject father, string propertyName, IJavascriptObject value)
        {
            father.SetValue(propertyName, value);
        }
    }
}
