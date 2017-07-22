using Neutronium.Core.Binding.GlueObject;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.Listeners
{
    public interface IUpdatableJSCSGlueCollection
    {
        ISet<IJSCSGlue> GetAllChildren();

        void OnEnter(IJSCSGlue item);

        void OnExit(IJSCSGlue item);
    }
}
