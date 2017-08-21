using MoreCollection.Extensions;
using Neutronium.Core.Extension;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.JavascriptFramework
{
    public class BinderBuilder
    {
        private readonly IWebView _WebView;
        private readonly IJavascriptChangesObserver _Observer;

        public BinderBuilder(IWebView webView, IJavascriptChangesObserver javascriptObserver)
        {
            _WebView = webView;
            _Observer = javascriptObserver;
        }

        public IJavascriptObject BuildListener()
        {
            var listener = _WebView.Factory.CreateObject();

            if (_Observer == null)
                return listener;

            listener.BindArguments("TrackChanges", _WebView, (first, second, third) => _Observer.OnJavaScriptObjectChanges(first, second.GetStringValue(), third));
            listener.BindArguments("TrackCollectionChanges", _WebView, JavascriptColectionChanged);

            return listener;
        }

        private void JavascriptColectionChanged(IJavascriptObject collectionArg, IJavascriptObject valuesArg, IJavascriptObject typesArg, IJavascriptObject indexesArg)
        {
            var values = valuesArg.GetArrayElements();
            var types = typesArg.GetArrayElements();
            var indexes = indexesArg.GetArrayElements();
            var collectionChange = new JavascriptCollectionChanges(collectionArg, values.Zip(types, indexes, (v, t, i) => new IndividualJavascriptCollectionChange(t.GetStringValue() == "added" ? CollectionChangeType.Add : CollectionChangeType.Remove, i.GetIntValue(), v)));

            _Observer.OnJavaScriptCollectionChanges(collectionChange);
        }
    }
}
