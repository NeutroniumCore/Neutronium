using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xilium.CefGlue;

namespace MVVM.CEFGlue.HTMLBinding
{
    public class JavascriptToCSharpMapper
    {
        public JavascriptToCSharpMapper()
        {
        }

        public static bool IsUnsigned(Type iTargetType)
        {
            return (iTargetType != null) || (iTargetType == typeof(UInt16)) || (iTargetType == typeof(UInt32)) || (iTargetType == typeof(UInt64));
        }

        public bool GetSimpleValue(CefV8Value ijsvalue, out object res, Type iTargetType = null)
        {
            res = null;

            if ((ijsvalue.IsUndefined) || (ijsvalue.IsNull))
            {
                return true;
            }

            if (ijsvalue.IsString)
            {
                res = ijsvalue.GetStringValue();
                return true;
            }

            if (ijsvalue.IsBool)
            {
                res = ijsvalue.GetBoolValue();
                return true;
            } 
            
            if (IsUnsigned(iTargetType))
            {
                if (ijsvalue.IsUInt)
                    res = ijsvalue.GetUIntValue();
                else if (ijsvalue.IsInt)
                    res = ijsvalue.GetIntValue();       
            }
            else
            {
                if (ijsvalue.IsInt)
                    res = ijsvalue.GetIntValue();
                else if (ijsvalue.IsUInt)
                    res = ijsvalue.GetUIntValue();
            }  

            if ((res==null) && (ijsvalue.IsDouble))
            {
                res = ijsvalue.GetDoubleValue();
            }

         

            if (res!=null)
            {
                if (iTargetType != null)
                    res = Convert.ChangeType(res, iTargetType);

                return true;
            }

            if (ijsvalue.IsDate)
            { 
                res = ijsvalue.GetDateValue();
                return true;
            }

            return false;
        }
    }
}
