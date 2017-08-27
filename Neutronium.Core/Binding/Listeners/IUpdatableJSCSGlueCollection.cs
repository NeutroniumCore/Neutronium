using Neutronium.Core.Binding.GlueObject;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.Listeners
{
    internal interface IUpdatableJsCsGlueCollection
    {
        ISet<IJsCsGlue> GetAllChildren();

        void CollectionChanged();

        void OnEnter(IJsCsGlue entering);

        void OnExit(IJsCsGlue exiting, BridgeUpdater context);
    }
}
