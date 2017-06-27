using Neutronium.Core.JavascriptFramework;
using System;

namespace Neutronium.Core.Binding.GlueObject
{
    public class BridgeUpdater
    {
        public Action<IJavascriptViewModelUpdater> UpdateJavascriptObject { get; }

        public BridgeUpdater(Action<IJavascriptViewModelUpdater> update)
        {
            UpdateJavascriptObject = update;
        }

        public BridgeUpdater()
        {
            UpdateJavascriptObject = _ => { };
        }
    }
}
