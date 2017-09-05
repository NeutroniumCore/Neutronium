using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Binding.Builder
{
    internal sealed class JSAllBuilderAdapter : IJavascriptObjectBuilder
    {
        private IJsCsGlue _Object;
        private readonly JavascriptObjectBulkBuilder _JavascriptObjectBuilder;

        public JSAllBuilderAdapter(JavascriptObjectBulkBuilder javascriptObjectBuilder)
        {
            _JavascriptObjectBuilder = javascriptObjectBuilder;
        }

        public bool Visit(IJsCsGlue @object)
        {
            if (@object.JsValue != null)
                return false;

            _Object = @object;
            _Object.RequestBuildInstruction(this);
            return true;
        }

        public void RequestArrayCreation(IList<IJsCsGlue> children)
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

        public void RequestExecutableCreation()
        {
            _JavascriptObjectBuilder.RequestExecutableCreation(_Object);
        }

        public void RequestObjectCreation(AttributeDescription[] children, bool updatableFromJs)
        {
            _JavascriptObjectBuilder.RequestObjectCreation(_Object, children, updatableFromJs);
        }
    }
}