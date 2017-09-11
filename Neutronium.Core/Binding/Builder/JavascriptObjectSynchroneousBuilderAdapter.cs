using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;
using System;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using MoreCollection.Extensions;
using Neutronium.Core.Exceptions;
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
        }

        public void ApplyLocalChanges()
        {
            _Object.RequestBuildInstruction(this);
        }

        public void AfterChildrenUpdates()
        {
            _AfterChildrenUpdates?.Invoke();
        }

        void IJavascriptObjectBuilder.RequestArrayCreation(IList<IJsCsGlue> children)
        {
            var value = _Factory.CreateArray(children?.Count ?? 0);
            SetValue(value);

            if (children != null)
                _AfterChildrenUpdates = () => children.ForEach((child, index) => value.SetValue(index, child.JsValue));
        }

        void IJavascriptObjectBuilder.RequestBasicObjectCreation(object @object)
        {
            if (@object == null)
            {
                _Object.SetJsValue(_Factory.CreateNull());
                return;
            }

            IJavascriptObject value;
            if (_Factory.CreateBasic(@object, out value))
            {
                _Object.SetJsValue(value);
                return;
            }

            if (!@object.GetType().IsEnum)
                throw ExceptionHelper.Get("Algorithm core unexpected behaviour");

            _Object.SetJsValue(_Factory.CreateEnum((Enum)@object));
            _Cache.Cache(_Object);
        }

        void IJavascriptObjectBuilder.RequestCommandCreation(bool canExecute)
        {
            var command = _Factory.CreateObject(true);
            command.SetValue("CanExecuteValue", _Factory.CreateBool(canExecute));
            command.SetValue("CanExecuteCount", _Factory.CreateInt(1));
            SetValue(command);

            UpdateExecutable(command);
        }

        void IJavascriptObjectBuilder.RequestExecutableCreation()
        {
            var executable = _Factory.CreateObject(true);
            SetValue(executable);

            UpdateExecutable(executable);
        }

        private void UpdateExecutable(IJavascriptObject @object)
        {
            if (_Mapping)
                return;

            var executable = _Object as IExecutableGlue;
            executable?.UpdateJsObject(@object);
        }

        void IJavascriptObjectBuilder.RequestObjectCreation(TypePropertyAccessor attributeDescription, IJsCsGlue[] attributeValue)
        {
            var value = _Factory.CreateObject(!attributeDescription.HasReadWriteProperties);
            SetValue(value);

            _AfterChildrenUpdates = () =>
            {
                var properties = attributeDescription.ReadProperties;
                for (var i = 0; i < properties.Length; i++)
                {
                    value.SetValue(properties[i].Name, attributeValue[i].JsValue);
                }
            };
        }

        private void SetValue(IJavascriptObject value)
        {
            _Object.SetJsValue(value);

            if (_Mapping)
                return;

            _Cache.Cache(_Object);
        }
    }
}