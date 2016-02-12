using System;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using Xilium.CefGlue;

namespace MVVM.Cef.Glue
{
    internal class CefV8_Converter : IJavascriptObjectConverter
    {
        public CefV8_Converter()
        {
        }

        public static bool IsUnsigned(Type iTargetType)
        {
            return (iTargetType != null) && ((iTargetType == typeof(UInt16)) || (iTargetType == typeof(UInt32)) || (iTargetType == typeof(UInt64)));
        }

        public bool GetSimpleValue(IJavascriptObject ijsvalue, out object res, Type iTargetType = null)
        {
            res = null;
            CefV8Value value = CefV8_JavascriptObject.Convert(ijsvalue);

            if ((value.IsUndefined) || (value.IsNull))
            {
                return true;
            }

            if (value.IsString)
            {
                res = ijsvalue.GetStringValue();
                return true;
            }

            if (value.IsBool)
            {
                res = value.GetBoolValue();
                return true;
            }

            if (IsUnsigned(iTargetType))
            {
                if (value.IsUInt)
                    res = value.GetUIntValue();
            }
            else
            {
                if (value.IsInt)
                    res = value.GetIntValue();
            }

            if ((res == null) && (value.IsDouble))
            {
                res = value.GetDoubleValue();
            }

            if (res != null)
            {
                if (iTargetType != null)
                    res = Convert.ChangeType(res, iTargetType);

                return true;
            }

            if (value.IsDate)
            {
                res = value.GetDateValue();
                return true;
            }

            return false;
        }
    }
}
