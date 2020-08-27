using MoreCollection.Extensions;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.JavascriptFramework
{
    internal class BinderBuilder
    {
        private readonly IWebView _WebView;
        private readonly IJavascriptChangesListener _Listener;

        internal BinderBuilder(IWebView webView, IJavascriptChangesListener javascriptListener)
        {
            _WebView = webView;
            _Listener = javascriptListener;
        }

        internal IJavascriptObject BuildListener()
        {
            var listener = _WebView.Factory.CreateObject();

            if (_Listener == null)
                return listener;

            listener.BindArguments("TrackChanges", _WebView, (first, second, third) => _Listener.OnJavaScriptObjectChanges(first, second.GetStringValue(), third));
            listener.BindArguments("TrackCollectionChanges", _WebView, JavascriptCollectionChanged);

            return listener;
        }

        private void JavascriptCollectionChanged(IJavascriptObject collectionArg, IJavascriptObject valuesArg, IJavascriptObject typesArg, IJavascriptObject indexesArg)
        {
            var values = valuesArg.GetArrayElements();
            var types = typesArg.GetArrayElements();
            var indexes = indexesArg.GetArrayElements();
            var collectionChange = new JavascriptCollectionChanges(collectionArg, values.Zip(types, indexes, (v, t, i) => new IndividualJavascriptCollectionChange(t.GetStringValue() == "added" ? CollectionChangeType.Add : CollectionChangeType.Remove, i.GetIntValue(), v)));

            _Listener.OnJavaScriptCollectionChanges(collectionChange);
        }
    }
}
