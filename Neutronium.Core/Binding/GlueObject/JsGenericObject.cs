using System;
using System.Collections.Generic;
using System.Linq;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.Listeners;
using System.ComponentModel;
using Neutronium.Core.Infra;

namespace Neutronium.Core.Binding.GlueObject
{
    internal class JsGenericObject : GlueBase, IJsCsCachableGlue
    {
        private AttributeDescription[] _Attributes;

        public virtual IJavascriptObject CachableJsValue => JsValue;
        public object CValue { get; }
        public JsCsGlueType Type => JsCsGlueType.Object;
        public IEnumerable<IJsCsGlue> Children => _Attributes.Select(at => at.Glue);

        public uint JsId { get; private set; }

        void IJsCsCachableGlue.SetJsId(uint jsId) => JsId = jsId;

        public JsGenericObject(object cValue)
        {
            CValue = cValue;
        }

        internal void SetAttributes(AttributeDescription[] attributes)
        {
            if (_Attributes == null)
            {
                _Attributes = attributes;
            }
            else
            {
                var newAttributes = _Attributes.Concat(attributes).OrderBy(at => at.Name).ToArray();
                _Attributes = newAttributes;
            }
        }

        public IJsCsGlue GetAttribute(string propertyName) => Get(propertyName).Glue;

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder)
        {
            var updatableFromJs = CValue.GetType().HasReadWriteProperties();
            builder.RequestObjectCreation(_Attributes, updatableFromJs);
        }

        public void VisitChildren(Func<IJsCsGlue, bool> visit)
        {
            if (!visit(this))
                return;

            foreach (var attribute in _Attributes)
            {
                attribute.Glue.VisitChildren(visit);
            }
        }

        protected override void ComputeString(DescriptionBuilder context)
        {
            context.Append("{");
            var first = true;
            foreach (var it in _Attributes)
            {
                if (!first)
                    context.Append(",");

                context.Append($@"""{it.Name}"":");

                using (context.PushContext(it.Name))
                {
                    it.Glue.BuilString(context);
                }

                first = false;
            }

            context.Append("}");
        }

        internal IJsCsGlue UpdateGlueProperty(string propertyName, IJsCsGlue glue)
        {
            glue.AddRef();
            var oldGlue = Get(propertyName).Glue;      
            Get(propertyName).Glue = glue;
            return oldGlue.Release()? oldGlue : null;
        }

        private AttributeDescription Get(string name)
        {
            var index = Array.BinarySearch(_Attributes, new AttributeDescription(name));
            return (index == -1) ? null : _Attributes[index];
        }

        public BridgeUpdater GetUpdater(string propertyName, IJsCsGlue glue)
        {
            var old = UpdateGlueProperty(propertyName, glue);
            return new BridgeUpdater(viewModelUpdater => viewModelUpdater?.UpdateProperty(CachableJsValue, propertyName,
                    glue.GetJsSessionValue(), !glue.IsBasic()))
                        .Remove(old);
        }

        public void ApplyOnListenable(IObjectChangesListener listener)
        {
            var notifyPropertyChanged = CValue as INotifyPropertyChanged;
            if (notifyPropertyChanged == null)
                return;

            listener.OnObject(notifyPropertyChanged);
        }
    }
}
