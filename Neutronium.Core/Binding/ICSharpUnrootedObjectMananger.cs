using System;
using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Binding
{
    internal interface ICSharpUnrootedObjectManager
    {
        void RegisterInSession(object nv, Action<IJsCsGlue> performAfterBuild);
    }
}
