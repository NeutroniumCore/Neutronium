using Neutronium.Core.Binding.GlueObject;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.Builder
{
    public interface IJavascriptObjectBuilder
    {
        void RequestObjectCreation(IReadOnlyDictionary<string, IJSCSGlue> children=null);

        void RequestArrayCreation(IList<IJSCSGlue> children);

        void RequestBasicObjectCreation(object @object);

        void RequestCommandCreation(bool canExcecute);
    }
}