using Neutronium.Core.Binding.GlueObject;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.Builder
{
    public interface IJavascriptObjectBuilder
    {
        void RequestObjectCreation(AttributeDescription[] children =null, bool updatableFromJs= false);

        void RequestArrayCreation(IList<IJsCsGlue> children);

        void RequestBasicObjectCreation(object @object);

        void RequestCommandCreation(bool canExecute);

        void RequestExecutableCreation();
    }
}