using System;
using System.Collections.Generic;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.Listeners;
using System.ComponentModel;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.Binding.GlueObject
{
    internal class JsGenericObject : GlueBase, IJsCsCachableGlue
    {
        private readonly TypePropertyAccessor _TypePropertyAccessor;
        private IJsCsGlue[] _Attributes;

        public virtual IJavascriptObject CachableJsValue => JsValue;
        public object CValue { get; }
        public JsCsGlueType Type => JsCsGlueType.Object;
        public IEnumerable<IJsCsGlue> Children => _Attributes;
        public bool HasReadWriteProperties => _TypePropertyAccessor.HasReadWriteProperties;
        public uint JsId { get; private set; }

        void IJsCsCachableGlue.SetJsId(uint jsId) => JsId = jsId;

        public JsGenericObject(object cValue, TypePropertyAccessor typePropertyAccessor)
        {
            _TypePropertyAccessor = typePropertyAccessor;
            CValue = cValue;
        }

        internal void SetAttributes(IJsCsGlue[] attributes)
        {
            _Attributes = attributes;
        }

        public AttibuteUpdater GetPropertyUpdater(string propertyName)
        {
            var propertyAcessor = GetPropertyAccessor(propertyName);
            return (propertyAcessor == null) ? new AttibuteUpdater(this, null, null) : new AttibuteUpdater(this, propertyAcessor, _Attributes[propertyAcessor.Position]);
        }

        public IJsCsGlue GetAttribute(string propertyName)
        {
            var propertyAcessor = GetPropertyAccessor(propertyName);
            return (propertyAcessor == null) ? null : _Attributes[propertyAcessor.Position];
        }

        private PropertyAccessor GetPropertyAccessor(string propertyName) => _TypePropertyAccessor.GetAccessor(propertyName);

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder)
        {
            builder.RequestObjectCreation(_TypePropertyAccessor, _Attributes);
        }

        public void VisitChildren(Func<IJsCsGlue, bool> visit)
        {
            if (!visit(this))
                return;

            foreach (var attribute in _Attributes)
            {
                attribute.VisitChildren(visit);
            }
        }

        protected override void ComputeString(DescriptionBuilder context)
        {
            context.Append("{");
            for (var i = 0; i < _Attributes.Length; i++)
            {
                if (i != 0)
                    context.Append(",");

                var name = _TypePropertyAccessor.AttributeNames[i];
                context.Append($@"""{name}"":");

                using (context.PushContext(name))
                {
                    _Attributes[i].BuilString(context);
                }
            }

            context.Append("}");
        }

        internal IJsCsGlue UpdateGlueProperty(AttibuteUpdater attributeDescription, IJsCsGlue glue)
        {
            var oldGlue = attributeDescription.Child;
            _Attributes[attributeDescription.Index] = glue.AddRef();
            return oldGlue.Release() ? oldGlue : null;
        }

        public BridgeUpdater GetUpdater(AttibuteUpdater propertyUpdater, IJsCsGlue glue)
        {
            var old = UpdateGlueProperty(propertyUpdater, glue);
            return new BridgeUpdater(viewModelUpdater => viewModelUpdater?.UpdateProperty(CachableJsValue, propertyUpdater.PropertyName,
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
