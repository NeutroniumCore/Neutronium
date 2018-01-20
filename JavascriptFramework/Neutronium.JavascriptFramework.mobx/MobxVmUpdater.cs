using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.JavascriptFramework.mobx
{
    public class MobxVmUpdater
    {
        internal IJavascriptObject ChangeAndInject { get; }
        internal IJavascriptObject Change { get; }
        internal IJavascriptObject AddProperty { get; }
        internal IJavascriptObject InjectDetached { get; }

        internal MobxVmUpdater(IJavascriptObject helper)
        {
            ChangeAndInject = helper.GetValue("silentChangeAndInject");
            Change = helper.GetValue("silentChange");
            AddProperty = helper.GetValue("addProperty");
            InjectDetached = helper.GetValue("injectDetached");
        }
    }
}
