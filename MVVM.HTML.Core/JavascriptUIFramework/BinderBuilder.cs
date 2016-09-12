using Neutronium.Core.Extension;
using Neutronium.Core.Infra;
using Neutronium.Core.JavascriptEngine.JavascriptObject;

namespace Neutronium.Core.JavascriptUIFramework
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
            return _WebView.Evaluate(() =>
            {
                var listener = _WebView.Factory.CreateObject(false);

                if (_Observer == null)
                    return listener;

                listener.Bind("TrackChanges", _WebView, (e) => _Observer.OnJavaScriptObjectChanges(e[0], e[1].GetStringValue(), e[2]));
                listener.Bind("TrackCollectionChanges", _WebView, JavascriptColectionChanged);

                return listener;
            });
        }

        private void JavascriptColectionChanged(IJavascriptObject[] arguments)
        {
            var values = arguments[1].GetArrayElements();
            var types = arguments[2].GetArrayElements();
            var indexes = arguments[3].GetArrayElements();
            var collectionChange = new JavascriptCollectionChanges(arguments[0], values.Zip(types, indexes, (v, t, i) => new IndividualJavascriptCollectionChange(t.GetStringValue() == "added" ? CollectionChangeType.Add : CollectionChangeType.Remove, i.GetIntValue(), v)));

            _Observer.OnJavaScriptCollectionChanges(collectionChange);
        }
    }
}
