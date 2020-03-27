using System;
using Chromium.Remote;
using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.WebBrowserEngine.ChromiumFx.Convertion;

namespace Neutronium.WebBrowserEngine.ChromiumFx.EngineBinding 
{
    internal class ChromiumFxConverter : IJavascriptObjectConverter
    {
        private readonly CfrV8Context _CfrV8Context;
        internal ChromiumFxConverter(CfrV8Context context) 
        {
            _CfrV8Context = context;
        }

        public bool GetSimpleValue(IJavascriptObject decoratedValue, out object res, Type targetType = null) 
        {
            res = null;
            var value = decoratedValue.Convert();

            if (value.IsObject)
            {
                return false;
            }

            if ((value.IsUndefined) || (value.IsNull)) 
            {
                return true;
            }

            if (value.IsString) 
            {
                res = value.StringValue;
                return true;
            }

            if (value.IsBool) 
            {
                res = value.BoolValue;
                return true;
            }

            if (targetType.IsUnsigned()) 
            {
                if (value.IsUint)
                    res = value.UintValue;
            }
            else 
            {
                if (value.IsInt)
                    res = value.IntValue;
            }

            if ((res == null) && (value.IsDouble)) 
            {
                res = value.DoubleValue;
            }

            if (res != null) 
            {
                if (targetType != null)
                    res = Convert.ChangeType(res, targetType);

                return true;
            }

            if (value.IsDate) 
            {
                var dateValue = value.DateValue;
                res = CfrTime.ToUniversalTime(dateValue);
                return true;
            }

            return false;
        }
    }
}
