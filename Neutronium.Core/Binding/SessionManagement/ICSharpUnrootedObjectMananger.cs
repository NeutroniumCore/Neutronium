using System;
using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Binding.SessionManagement
{
    public interface ICSharpUnrootedObjectManager: IDisposable
    {
        void RegisterInSession(object nv, Action<IJsCsGlue> performAfterBuild);
    }
}
