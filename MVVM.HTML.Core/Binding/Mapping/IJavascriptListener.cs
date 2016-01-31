using MVVM.HTML.Core.V8JavascriptObject;

namespace MVVM.HTML.Core.HTMLBinding
{
    public interface IJavascriptListener
    {
        void OnJavaScriptObjectChanges(IJavascriptObject objectchanged, string PropertyName, IJavascriptObject newValue);

        void OnJavaScriptCollectionChanges(JavascriptCollectionChanges changes);
    }
}
