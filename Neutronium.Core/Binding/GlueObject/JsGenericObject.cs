using System;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.Listeners;
using System.ComponentModel;
using Neutronium.Core.Infra.Reflection;
using System.Collections.Generic;
using Neutronium.Core.Infra;

namespace Neutronium.Core.Binding.GlueObject
{
    internal class JsGenericObject : GlueBase, IJsCsCachableGlue
    {
        private readonly IGenericPropertyAcessor _TypePropertyAccessor;
        private IReadOnlyList<IJsCsGlue> _Attributes;

        public virtual IJavascriptObject CachableJsValue => JsValue;
        public object CValue { get; }
        public JsCsGlueType Type => JsCsGlueType.Object;
        public bool HasReadWriteProperties => _TypePropertyAccessor.HasReadWriteProperties;
        public uint JsId { get; private set; }

        void IJsCsCachableGlue.SetJsId(uint jsId) => JsId = jsId;

        public JsGenericObject(object cValue, IGenericPropertyAcessor typePropertyAccessor)
        {
            _TypePropertyAccessor = typePropertyAccessor;
            CValue = cValue;
        }

        public virtual void SetJsValue(IJavascriptObject value, IJavascriptSessionCache sessionCache)
        {
            SetJsValue(value);
            sessionCache.Cache(this);
        }

        internal void SetAttributes(IReadOnlyList<IJsCsGlue> attributes)
        {
            _Attributes = attributes;
        }

        public AttibuteUpdater GetPropertyUpdater(string propertyName)
        {
            var propertyAcessor = GetPropertyAccessor(propertyName);
            return new AttibuteUpdater(this, propertyAcessor, GetGlueFromPropertyAccessor(propertyAcessor));
        }

        public IJsCsGlue GetAttribute(string propertyName)
        {
            var propertyAcessor = GetPropertyAccessor(propertyName);
            return GetGlueFromPropertyAccessor(propertyAcessor);
        }

        private IJsCsGlue GetGlueFromPropertyAccessor(PropertyAccessor propertyAccessor)
        {
            return ((propertyAccessor == null) || (propertyAccessor.Position == -1)) ? null : _Attributes[propertyAccessor.Position];
        }

        private PropertyAccessor GetPropertyAccessor(string propertyName) => _TypePropertyAccessor.GetAccessor(propertyName);

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder)
        {
            builder.RequestObjectCreation(_TypePropertyAccessor, _Attributes);
        }

        public void VisitDescendants(Func<IJsCsGlue, bool> visit)
        {
            if (!visit(this))
                return;

            foreach (var attribute in _Attributes)
            {
                attribute.VisitDescendants(visit);
            }
        }

        public void VisitChildren(Action<IJsCsGlue> visit) 
        {
            foreach (var item in _Attributes)
                visit(item);
        }

        protected override void ComputeString(DescriptionBuilder context)
        {
            context.Append("{");
            for (var i = 0; i < _Attributes.Count; i++)
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
            return PrivateUpdateGlueProperty(attributeDescription, glue).ToBeCleaned;
        }

        private UpdateInformation PrivateUpdateGlueProperty(AttibuteUpdater attributeDescription, IJsCsGlue glue) 
        {
            var oldGlue = attributeDescription.Child;
            var index = _TypePropertyAccessor.GetIndex(attributeDescription.PropertyAccessor);
            _Attributes.Apply(index, glue.AddRef());
            return new UpdateInformation { AddedProperty = index.Insert, ToBeCleaned = (oldGlue?.Release() == true) ? oldGlue : null };
        }

        private struct UpdateInformation 
        {
            public IJsCsGlue ToBeCleaned { get; set; }
            public bool AddedProperty { get; set; }
        }

        public BridgeUpdater GetUpdater(AttibuteUpdater propertyUpdater, IJsCsGlue glue)
        {
            var old = UpdateGlueProperty(propertyUpdater, glue);
            return new BridgeUpdater(viewModelUpdater => viewModelUpdater.UpdateProperty(CachableJsValue, propertyUpdater.PropertyName,
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
