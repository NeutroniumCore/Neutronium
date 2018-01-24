using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.JavascriptFramework.mobx
{
    public class MobxVmUpdater
    {
        internal IJavascriptObject SilentChange { get; }
        internal IJavascriptObject SilentChangeUpdate { get; }
        internal IJavascriptObject SilentSplice { get; }
        internal IJavascriptObject UpdateVm { get; }
        internal IJavascriptObject UnListen { get; }
        internal IJavascriptObject ClearCollection { get; }
        internal IJavascriptObject AddProperty { get; }

        internal MobxVmUpdater(IJavascriptObject helper)
        {
            AddProperty = helper.GetValue("addProperty");
            SilentChange = helper.GetValue("silentChange");
            SilentChangeUpdate = helper.GetValue("silentChangeUpdate");
            SilentSplice = helper.GetValue("silentSplice");
            UpdateVm = helper.GetValue("updateVm");
            UnListen = helper.GetValue("unListen");
            ClearCollection = helper.GetValue("clearCollection");
        }
    }
}
