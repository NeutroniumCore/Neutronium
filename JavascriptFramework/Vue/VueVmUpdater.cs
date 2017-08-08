using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.JavascriptFramework.Vue
{
    internal class VueVmUpdater
    {
        internal IJavascriptObject ChangeAndInject { get; private set; }
        internal IJavascriptObject Change { get; private set; }

        internal VueVmUpdater(IJavascriptObject helper)
        {
            ChangeAndInject = helper.GetValue("silentChangeAndInject");
            Change = helper.GetValue("silentChange");
        }
    }
}
