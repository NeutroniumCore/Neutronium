using System.ComponentModel;
using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Binding.Updaters
{
    internal interface IJsUpdater 
    {
        void CheckUiContext();
        JsGenericObject GetCached(object value);
        IJsCsGlue Map(object value);
        BridgeUpdater UpdateBridgeUpdater(BridgeUpdater value);
        void UpdateOnJavascriptContextAllContext(BridgeUpdater updater, IJsCsGlue value);
        IJavascriptUpdater GetCSharpPropertyChanged(object sender, PropertyChangedEventArgs e);
    }
}