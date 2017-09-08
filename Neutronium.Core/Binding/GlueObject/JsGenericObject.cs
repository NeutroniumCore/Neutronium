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
            _Attributes = attributes;
        }

        internal void Merge(JsGenericObject other) 
        {
            if (other == null)
                return;
            var newAttributes = _Attributes.Concat(other._Attributes).OrderBy(at => at.Name).ToArray();
            _Attributes = newAttributes;
        }

        public IJsCsGlue GetAttribute(string propertyName) => GetAttributeDescription(propertyName).Glue;

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

        private static IJsCsGlue UpdateGlueProperty(AttributeDescription attributeDescription, IJsCsGlue glue)
        {
            var oldGlue = attributeDescription.Glue;
            attributeDescription.Glue = glue.AddRef();
            return oldGlue.Release()? oldGlue : null;
        }

        internal IJsCsGlue UpdateGlueProperty(string name, IJsCsGlue glue) 
        {
            var description = GetAttributeDescription(name);
            return UpdateGlueProperty(description, glue);
        }

        internal AttributeDescription GetAttributeDescription(string name)
        {
            var index = Array.BinarySearch(_Attributes, name, AttributeDescription.Comparer);
            return (index < 0) ? null : _Attributes[index];
        }

        public BridgeUpdater GetUpdater(AttributeDescription attributeDescription, IJsCsGlue glue)
        {
            var old = UpdateGlueProperty(attributeDescription, glue);
            return new BridgeUpdater(viewModelUpdater => viewModelUpdater?.UpdateProperty(CachableJsValue, attributeDescription.Name,
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
