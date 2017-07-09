using Neutronium.Core.Binding.GlueObject;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.Builder
{
    public interface IJavascriptObjectOneShotBuilder
    {
        void RequestArrayCreation(IJSCSGlue glue, IList<IJSCSGlue> children);

        void RequestBasicObjectCreation(IJSCSGlue glue, object @object);

        void RequestCommandCreation(IJSCSGlue glue, bool canExcecute);

        void RequestObjectCreation(IJSCSGlue glue, IReadOnlyDictionary<string, IJSCSGlue> children = null);

        void UpdateJavascriptValue();
    }
}
