using MoreCollection.Extensions;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Exceptions;
using Neutronium.Core.Extension;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Binding.Builder
{
    internal class JavascriptObjectSynchroneousBuilder : IJavascriptObjectOneShotBuilder
    {
        private readonly IJavascriptObjectFactory _Factory;
        private readonly IJavascriptSessionCache _Cache;
        private readonly IJSCSGlue _Root;

        private readonly List<Action> _Updates = new List<Action>();

        public JavascriptObjectSynchroneousBuilder(IJavascriptObjectFactory factory, IJavascriptSessionCache cache, IJSCSGlue root)
        {
            _Factory = factory;
            _Cache = cache;
            _Root = root;
        }

        public void UpdateJavascriptValue()
        {
            var builders = _Root.GetAllChildren(true).Where(glue => glue.JSValue == null)
                                .Select(glue => new JSBuilderAdapter(glue, this)).ToList();

            builders.ForEach(builder => builder.GetBuildRequest());
            UpdateDependencies();
        }

        private void UpdateDependencies()
        {
            _Updates.ForEach(@action => @action());
        }

        void IJavascriptObjectOneShotBuilder.RequestObjectCreation(IJSCSGlue glue, IReadOnlyDictionary<string, IJSCSGlue> children)
        {
            var value = _Factory.CreateObject(true);
            glue.SetJSValue(value);
            if (children != null)
                _Updates.Add(() => children.ForEach((child) => value.SetValue(child.Key, child.Value.JSValue)));
        }

        void IJavascriptObjectOneShotBuilder.RequestArrayCreation(IJSCSGlue glue, IList<IJSCSGlue> children)
        {
            var value = _Factory.CreateArray(children?.Count ?? 0);
            glue.SetJSValue(value);
            if (children != null)
                _Updates.Add(() => children.ForEach((child, index) => value.SetValue(index, child.JSValue)));
        }

        void IJavascriptObjectOneShotBuilder.RequestBasicObjectCreation(IJSCSGlue glueObject, object cValue)
        {
            if (cValue == null)
            {
                glueObject.SetJSValue(_Factory.CreateNull());
                return;
            }

            IJavascriptObject value;
            if (_Factory.CreateBasic(cValue, out value))
            {
                glueObject.SetJSValue(value);
                return;
            }

            if (!cValue.GetType().IsEnum)
                throw ExceptionHelper.Get("Algorithm core unexpected behaviour");

            glueObject.SetJSValue(_Factory.CreateEnum((Enum)cValue));
            _Cache.CacheLocal(cValue, glueObject);
        }

        void IJavascriptObjectOneShotBuilder.RequestCommandCreation(IJSCSGlue glueObject, bool canExcecute)
        {
            var command = _Factory.CreateObject(true);
            command.SetValue("CanExecuteValue", _Factory.CreateBool(canExcecute));
            command.SetValue("CanExecuteCount", _Factory.CreateInt(1));

            glueObject.SetJSValue(command);
        }
    }
}
