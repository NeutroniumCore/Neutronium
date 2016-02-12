using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core.HTMLBinding
{
    public interface IJavascriptChangesListener
    {
        void OnJavaScriptObjectChanges(IJavascriptObject objectchanged, string propertyName, IJavascriptObject newValue);

        void OnJavaScriptCollectionChanges(JavascriptCollectionChanges changes);
    }
}
