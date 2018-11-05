using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;
using System;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.Extension;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.Binding.Builder
{
    internal class JavascriptObjectSynchroneousBuilderAdapter : IJavascriptObjectBuilder
    {
        private readonly IJsCsGlue _Object;
        private readonly IJavascriptObjectFactory _Factory;
        private readonly IJavascriptSessionCache _Cache;
        private readonly bool _Mapping;
        private Action _AfterChildrenUpdates;

        public JavascriptObjectSynchroneousBuilderAdapter(IJavascriptObjectFactory factory, IJavascriptSessionCache cache, IJsCsGlue @object, bool mapping)
        {
            _Mapping = mapping;
            _Factory = factory;
            _Cache = cache;
            _Object = @object;
            _AfterChildrenUpdates = null;
        }

        public void ApplyLocalChanges()
        {
            _Object.RequestBuildInstruction(this);
        }

        public void AfterChildrenUpdates()
        {
            _AfterChildrenUpdates?.Invoke();
        }

        void IJavascriptObjectBuilder.RequestArrayCreation(IReadOnlyList<IJsCsGlue> children)
        {
            var value = _Factory.CreateArray(children?.Count ?? 0);
            SetJsValue(value);

            if (children == null)
                return;

            _AfterChildrenUpdates = () =>
            {
                for (var index = 0; index < children.Count; index++)
                {
                    value.SetValue(index, children[index].JsValue);
                }
            };
        }

        public void RequestNullCreation() => SetJsValue(_Factory.CreateNull());
        public void RequestIntCreation(int value) => SetJsValue(_Factory.CreateInt(value));
        public void RequestStringCreation(string value) => SetJsValue(_Factory.CreateString(value));
        public void RequestUintCreation(uint value) => SetJsValue(_Factory.CreateUint(value));
        public void RequestEnumCreation(Enum value) => SetJsValue(_Factory.CreateEnum(value));
        public void RequestBoolCreation(bool value) => SetJsValue(_Factory.CreateBool(value));
        public void RequestDoubleCreation(double value) => SetJsValue(_Factory.CreateDouble(value));
        public void RequestJsDateTimeCreation(DateTime value) => SetJsValue(_Factory.CreateDateTime(value));

        private void SetJsValue(IJavascriptObject jsObject)
        {
            _Object.SetJsValue(jsObject, _Cache);
        }

        void IJavascriptObjectBuilder.RequestCommandCreation(bool canExecute)
        {
            var command = _Factory.CreateObject(ObjectObservability.ReadOnly);
            command.SetValue("CanExecuteValue", _Factory.CreateBool(canExecute));
            command.SetValue("CanExecuteCount", _Factory.CreateInt(1));
            SetJsValue(command);

            UpdateExecutable(command);
        }

        void IJavascriptObjectBuilder.RequestExecutableCreation()
        {
            var executable = _Factory.CreateObject(ObjectObservability.ReadOnly);
            SetJsValue(executable);

            UpdateExecutable(executable);
        }

        private void UpdateExecutable(IJavascriptObject @object)
        {
            if (_Mapping)
                return;

            var executable = _Object as IExecutableGlue;
            executable?.UpdateJsObject(@object);
        }

        void IJavascriptObjectBuilder.RequestObjectCreation(IGenericPropertyAcessor attributeDescription, IReadOnlyList<IJsCsGlue> attributeValue)
        {
            var value = _Factory.CreateObject(attributeDescription.Observability);
            SetJsValue(value);

            _AfterChildrenUpdates = () =>
            {
                var properties = attributeDescription.ReadProperties;
                var count = properties.Count;
                for (var i = 0; i < count; i++)
                {
                    value.SetValue(properties[i].Name, attributeValue[i].JsValue);
                }
            };
        }
    }
}