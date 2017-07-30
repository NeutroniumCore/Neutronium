using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Binding.Builder
{
    internal class JSBuilderAdapter : IJavascriptObjectBuilder
    {
        private readonly IJSCSGlue _Object;
        private readonly JavascriptObjectBulkBuilder _JavascriptObjectBuilder;

        public JSBuilderAdapter(IJSCSGlue @object, JavascriptObjectBulkBuilder javascriptObjectBuilder)
        {
            _Object = @object;
            _JavascriptObjectBuilder = javascriptObjectBuilder;
        }

        public void GetBuildRequest()
        {
            _Object.RequestBuildInstruction(this);
        }

        public void RequestArrayCreation(IList<IJSCSGlue> children)
        {
            _JavascriptObjectBuilder.RequestArrayCreation(_Object, children);
        }

        public void RequestBasicObjectCreation(object @object)
        {
            _JavascriptObjectBuilder.RequestBasicObjectCreation(_Object, @object);
        }

        public void RequestCommandCreation(bool canExcecute)
        {
            _JavascriptObjectBuilder.RequestCommandCreation(_Object, canExcecute);
        }

        public void RequestObjectCreation(IReadOnlyDictionary<string, IJSCSGlue> children, bool updatableFromJS)
        {
            _JavascriptObjectBuilder.RequestObjectCreation(_Object, children, updatableFromJS);
        }
    }
}