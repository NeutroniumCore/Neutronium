using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace IntegratedTest.Infra.Windowless
{
    public interface IJavascriptFrameworkExtractor
    {
        IJavascriptObject GetAttribute(IJavascriptObject value, string attibutename);

        string GetStringAttribute(IJavascriptObject value, string attibutename);

        int GetIntAttribute(IJavascriptObject value, string attibutename);

        double GetDoubleAttribute(IJavascriptObject value, string attibutename);

        bool GetBoolAttribute(IJavascriptObject value, string attibutename);
    }
}
