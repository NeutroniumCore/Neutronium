using Neutronium.Core.Binding.GlueObject;
using System.Collections.Generic;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.Binding.Builder
{
    public interface IJavascriptObjectBuilder
    {
        void RequestObjectCreation(TypePropertyAccessor attributeDescription, IJsCsGlue[] attributeValue);

        void RequestArrayCreation(IList<IJsCsGlue> children);

        void RequestBasicObjectCreation(object @object);

        void RequestBasicObjectCreation(int value);

        void RequestCommandCreation(bool canExecute);

        void RequestExecutableCreation();
    }
}