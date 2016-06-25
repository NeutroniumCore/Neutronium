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
            _WebView.RunAsync(() =>
            {
                var length = array.GetArrayLength();
                array.Invoke("splice", _WebView, _WebView.Factory.CreateInt(0), _WebView.Factory.CreateInt(length));
            });
        }

        public void Dispose()
        {
        }

        public void MoveCollectionItem(IJavascriptObject array, IJavascriptObject item, int oldIndex, int newIndex)
        {
            SpliceCollection(array, oldIndex, 1);
            SpliceCollection(array, newIndex, 0, item);
        }

        public void SpliceCollection(IJavascriptObject array, int index, int number)
        {
            array.InvokeAsync("splice", _WebView, _WebView.Factory.CreateInt(index), _WebView.Factory.CreateInt(number));
        }

        public void SpliceCollection(IJavascriptObject array, int index, int number, IJavascriptObject added)
        {
            array.InvokeAsync("splice", _WebView, _WebView.Factory.CreateInt(index), _WebView.Factory.CreateInt(number), added);
        }

        public void UpdateProperty(IJavascriptObject father, string propertyName, IJavascriptObject value)
        {
            father.SetValue(propertyName, value);
        }
    }
}
