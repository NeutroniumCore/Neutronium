using Neutronium.Core.Binding.GlueObject;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.Listeners
{
    internal interface IUpdatableJSCSGlueCollection
    {
        ISet<IJSCSGlue> GetAllChildren();

        void OnEnter(IJSCSGlue entering);

        void OnExit(IJSCSGlue exiting, BridgeUpdater context);
    }
}
