using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;
using System;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using MoreCollection.Extensions;
using Neutronium.Core.Exceptions;
using Neutronium.Core.Extension;

namespace Neutronium.Core.Binding.Builder
{
    internal class JavascriptObjectSynchroneousBuilderAdapter : IJavascriptObjectBuilder
    {
        private readonly IJSCSGlue _Object;
        private readonly IJavascriptObjectFactory _Factory;
        private readonly IJavascriptSessionCache _Cache;
        private readonly bool _NeedToCacheObject;
        private Action _AfterChildrenUpdates;

        public JavascriptObjectSynchroneousBuilderAdapter(IJavascriptObjectFactory factory, IJavascriptSessionCache cache, IJSCSGlue @object, bool needToCacheObject)
        {
            _NeedToCacheObject = needToCacheObject;
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

        void IJavascriptObjectBuilder.RequestArrayCreation(IList<IJSCSGlue> children)
        {
            var value = _Factory.CreateArray(children?.Count ?? 0);
            SetValue(value);

            if (children != null)
                _AfterChildrenUpdates = () => children.ForEach((child, index) => value.SetValue(index, child.JSValue));
        }

        void IJavascriptObjectBuilder.RequestBasicObjectCreation(object @object)
        {
            if (@object == null)
            {
                _Object.SetJSValue(_Factory.CreateNull());
                return;
            }

            IJavascriptObject value;
            if (_Factory.CreateBasic(@object, out value))
            {
                _Object.SetJSValue(value);
                return;
            }

            if (!@object.GetType().IsEnum)
                throw ExceptionHelper.Get("Algorithm core unexpected behaviour");

            _Object.SetJSValue(_Factory.CreateEnum((Enum)@object));
            _Cache.Cache(_Object);
        }

        void IJavascriptObjectBuilder.RequestCommandCreation(bool canExcecute)
        {
            var command = _Factory.CreateObject(true);
            command.SetValue("CanExecuteValue", _Factory.CreateBool(canExcecute));
            command.SetValue("CanExecuteCount", _Factory.CreateInt(1));

            SetValue(command);
        }

        void IJavascriptObjectBuilder.RequestObjectCreation(IReadOnlyDictionary<string, IJSCSGlue> children, bool updatableFromJS)
        {
            var value = _Factory.CreateObject(!updatableFromJS);
            SetValue(value);

            if (children != null)
                _AfterChildrenUpdates = () => children.ForEach((child) => value.SetValue(child.Key, child.Value.JSValue));
        }

        private void SetValue(IJavascriptObject value)
        {
            _Object.SetJSValue(value);

            if (!_NeedToCacheObject)
                return;

            _Cache.Cache(_Object);
        }
    }
}