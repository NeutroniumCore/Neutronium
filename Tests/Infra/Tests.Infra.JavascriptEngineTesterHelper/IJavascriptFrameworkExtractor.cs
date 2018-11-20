using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Tests.Infra.JavascriptFrameworkTesterHelper
{
    public interface IJavascriptFrameworkExtractor
    {
        bool SupportDynamicBinding { get; }

        IJavascriptObject GetRootViewModel();

        IJavascriptObject GetAttribute(IJavascriptObject value, string attibutename);

        IJavascriptObject GetCollectionAttribute(IJavascriptObject value, string attibutename);

        void SetAttribute(IJavascriptObject father, string attibutename, IJavascriptObject value);

        void AddAttribute(IJavascriptObject father, string attibutename, IJavascriptObject value);

        string GetStringAttribute(IJavascriptObject value, string attibutename);

        int GetIntAttribute(IJavascriptObject value, string attibutename);

        double GetDoubleAttribute(IJavascriptObject value, string attibutename);

        bool GetBoolAttribute(IJavascriptObject value, string attibutename);
    }
}
