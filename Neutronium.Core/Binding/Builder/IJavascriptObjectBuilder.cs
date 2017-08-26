using Neutronium.Core.Binding.GlueObject;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.Builder
{
    public interface IJavascriptObjectBuilder
    {
        void RequestObjectCreation(ICollection<KeyValuePair<string, IJSCSGlue>> children=null, bool updatableFromJs= false);

        void RequestArrayCreation(IList<IJSCSGlue> children);

        void RequestBasicObjectCreation(object @object);

        void RequestCommandCreation(bool canExecute);

        void RequestExecutableCreation();
    }
}