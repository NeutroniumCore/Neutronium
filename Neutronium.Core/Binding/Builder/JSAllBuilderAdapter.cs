using System;
using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.Binding.Builder
{
    internal sealed class JsAllBuilderAdapter : IJavascriptObjectBuilder
    {
        private IJsCsGlue _Object;
        private readonly JavascriptObjectBulkBuilder _JavascriptObjectBuilder;

        public JsAllBuilderAdapter(JavascriptObjectBulkBuilder javascriptObjectBuilder)
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

        public void RequestArrayCreation(IReadOnlyList<IJsCsGlue> children)
        {
            _JavascriptObjectBuilder.RequestArrayCreation(_Object, children);
        }

        public void RequestNullCreation() => RequestCodeCreation();
        public void RequestBoolCreation(bool value) => RequestCodeCreation();
        public void RequestIntCreation(int value) => RequestCodeCreation();
        public void RequestStringCreation(string value) => RequestCodeCreation();
        public void RequestUintCreation(uint typedValue) => RequestCodeCreation();
        public void RequestEnumCreation(Enum value) => RequestCodeCreation();
        public void RequestDoubleCreation(double typedValue) => RequestCodeCreation();
        public void RequestJsDateTimeCreation(DateTime value) => RequestCodeCreation();

        private void RequestCodeCreation()
        {
            _JavascriptObjectBuilder.RequestBasicObjectCreation(_Object);
        }

        public void RequestCommandCreation(bool canExcecute)
        {
            _JavascriptObjectBuilder.RequestCommandCreation(_Object, canExcecute);
        }

        public void RequestExecutableCreation()
        {
            _JavascriptObjectBuilder.RequestExecutableCreation(_Object);
        }

        public void RequestObjectCreation(IGenericPropertyAcessor attributeDescription, IReadOnlyList<IJsCsGlue> attributeValue)
        {
            _JavascriptObjectBuilder.RequestObjectCreation(_Object, attributeDescription, attributeValue);
        }
    }
}