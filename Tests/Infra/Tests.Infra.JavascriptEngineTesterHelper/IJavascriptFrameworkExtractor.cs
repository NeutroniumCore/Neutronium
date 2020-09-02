using System.Threading.Tasks;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Tests.Infra.JavascriptFrameworkTesterHelper
{
    public interface IJavascriptFrameworkExtractor
    {
        bool SupportDynamicBinding { get; }

        IJavascriptObject GetRootViewModel();

        IJavascriptObject GetAttribute(IJavascriptObject value, string attributeName);

        Task<IJavascriptObject> GetAttributeAsync(IJavascriptObject value, string attributeName);

        IJavascriptObject GetCollectionAttribute(IJavascriptObject value, string attributeName);

        Task<IJavascriptObject> GetCollectionAttributeAsync(IJavascriptObject value, string attributeName);

        Task SetAttributeAsync(IJavascriptObject father, string attributeName, IJavascriptObject value);

        void AddAttribute(IJavascriptObject father, string attributeName, IJavascriptObject value);

        string GetStringAttribute(IJavascriptObject value, string attributeName);

        int GetIntAttribute(IJavascriptObject value, string attributeName);

        double GetDoubleAttribute(IJavascriptObject value, string attributeName);

        bool GetBoolAttribute(IJavascriptObject value, string attributeName);
    }
}
