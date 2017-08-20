using System.Collections.Generic;
using System.Linq;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using MoreCollection.Dictionary;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.Listeners;
using System.ComponentModel;
using Neutronium.Core.Infra;

namespace Neutronium.Core.Binding.GlueObject
{
    internal class JsGenericObject : GlueBase, IJSCSCachableGlue
    {
        private readonly IDictionary<string, IJSCSGlue> _Attributes;

        public virtual IJavascriptObject CachableJSValue => JSValue;
        public object CValue { get; }
        public JsCsGlueType Type => JsCsGlueType.Object;

        private uint _JsId;
        public uint JsId => _JsId;
        void IJSCSCachableGlue.SetJsId(uint jsId) => _JsId = jsId;

        public JsGenericObject(object cValue, int childrenCount)
        {
            CValue = cValue;           
            _Attributes = DictionaryFactory.Get<string, IJSCSGlue>(childrenCount);
        }

        public IJSCSGlue GetAttribute(string propertyName) => _Attributes[propertyName];

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder)
        {
            var updatableFromJS = CValue.GetType().HasReadWriteProperties();
            builder.RequestObjectCreation(_Attributes, updatableFromJS);
        }

        protected override void ComputeString(DescriptionBuilder context)
        {
            context.Append("{");
            var first = true;
            foreach (var it in _Attributes.OrderBy(kvp =>kvp.Key))
            {
                if (!first)
                    context.Append(",");

                context.Append($@"""{it.Key}"":");

                using (context.PushContext(it.Key))
                {                   
                    it.Value.BuilString(context);
                }

                first = false;
            }

            context.Append("}");
        }

        public IEnumerable<IJSCSGlue> GetChildren()
        {
            return _Attributes.Values; 
        }

        public void AddGlueProperty(string propertyName, IJSCSGlue glue)
        {
            _Attributes.Add(propertyName, glue);
        }

        public void UpdateGlueProperty(string propertyName, IJSCSGlue glue)
        {
            _Attributes[propertyName] = glue;
        }

        public BridgeUpdater GetUpdater(string propertyName, IJSCSGlue glue)
        {
            UpdateGlueProperty(propertyName, glue);
            return new BridgeUpdater(viewModelUpdater => viewModelUpdater?.UpdateProperty(CachableJSValue, propertyName, glue.GetJSSessionValue(), !glue.IsBasic()));
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
