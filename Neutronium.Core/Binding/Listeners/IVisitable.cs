using Neutronium.Core.Binding.GlueObject;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.Listeners
{
    public interface IVisitable
    {
        void Visit(IListenableObjectVisitor visitor);

        HashSet<IJSCSGlue> GetAllChildren();
    }
}
