using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Input;

using Xilium.CefGlue;

using MVVM.CEFGlue.Infra;
using MVVM.CEFGlue.CefGlueHelper;



namespace MVVM.CEFGlue.HTMLBinding
{
    public class JSGenericObject : GlueBase, IJSObservableBridge
    {
        private CefV8Context _CefV8Context;
        public JSGenericObject(CefV8Context context, CefV8Value value, object icValue)
        {
            JSValue = value;
            CValue = icValue;
            _CefV8Context = context;
        }

        private JSGenericObject(CefV8Context context,CefV8Value value)
        {
            JSValue = value;
            _MappedJSValue = value;
            CValue = null;
            _CefV8Context = context;
        }

        public static JSGenericObject CreateNull(CefV8Context context, IJSOLocalBuilder builder)
        {
            return new JSGenericObject(context, builder.CreateNull());
        }


        protected override void ComputeString(StringBuilder sb, HashSet<IJSCSGlue> alreadyComputed)
        {
            sb.Append("{");

            bool f = true;
            foreach (var it in _Attributes.Where(kvp => kvp.Value.Type != JSCSGlueType.Command))
            {
                if (!f)
                    sb.Append(",");

                sb.Append(string.Format(@"""{0}"":", it.Key));

                f = false;
                it.Value.BuilString(sb, alreadyComputed);
            }

            sb.Append("}");
        }

        private Dictionary<string, IJSCSGlue> _Attributes = new Dictionary<string, IJSCSGlue>();

        public IDictionary<string, IJSCSGlue> Attributes { get { return _Attributes; } }

        public CefV8Value JSValue { get; private set; }

        private CefV8Value _MappedJSValue;

        public CefV8Value MappedJSValue { get { return _MappedJSValue; } }

        public void SetMappedJSValue(CefV8Value ijsobject, IJSCBridgeCache mapper)
        {
            _MappedJSValue = ijsobject;
        }

        private IDictionary<string, CefV8Value> _Silenters = new Dictionary<string, CefV8Value>(); 

        public object CValue { get; private set; }

        public JSCSGlueType Type { get { return JSCSGlueType.Object; } }

        public IEnumerable<IJSCSGlue> GetChildren()
        {
            return _Attributes.Values; 
        }

        public void UpdateCSharpProperty(string PropertyName, IJSCBridgeCache converter, CefV8Value newValue)
        {
            PropertyInfo propertyInfo = CValue.GetType().GetProperty(PropertyName, BindingFlags.Public | BindingFlags.Instance);
            if (!propertyInfo.CanWrite)
                return;

            var type = propertyInfo.PropertyType.GetUnderlyingNullableType() ?? propertyInfo.PropertyType;
            IJSCSGlue glue = converter.GetCachedOrCreateBasic(newValue, type);
            _Attributes[PropertyName] = glue;
            propertyInfo.SetValue(CValue, glue.CValue, null);
        }

        

        public void Reroot(string PropertyName, IJSCSGlue newValue)
        { 
            _Attributes[PropertyName]=newValue;

            CefV8Value silenter = null;
            if ( _Silenters.TryGetValue(PropertyName,out silenter))
            {
                silenter.InvokeAsync("silent", _CefV8Context, newValue.GetJSSessionValue());      
            }
            else
            {
                //WebCore.QueueWork(() =>
                //    {

                _CefV8Context.CreateInContextAsync( ()=>
                    {
                        var jso = _MappedJSValue;
                        if (!_Silenters.TryGetValue(PropertyName, out silenter))
                        {
                            silenter = jso.GetValue(PropertyName);
                            _Silenters.Add(PropertyName, silenter);
                        }
                        silenter.Invoke("silent", _CefV8Context, newValue.GetJSSessionValue());
                    });
            }
        }     
    }
}
