using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xilium.CefGlue;

using MVVM.CEFGlue.CefGlueHelper;

namespace MVVM.CEFGlue.HTMLBinding
{
    public class GlobalBuilder : IGlobalBuilder
    {
        private static uint _Count = 0;
        private string _NameScape;
        private CefV8CompleteContext _CefV8Context;

        public GlobalBuilder(CefV8CompleteContext iWebView, string iNameScape)
        {
            _CefV8Context = iWebView;
            _NameScape = iNameScape;
        }

        public CefV8Value CreateJSO()
        {
            return _CefV8Context.Evaluate(() =>
                {
                    var res = CefV8Value.CreateObject(null);
                    res.SetValue("_globalId_", CefV8Value.CreateUInt(++_Count), CefV8PropertyAttribute.DontDelete);
                    return res;
                });
        }


        public uint GetID(CefV8Value iJSObject)
        {
            return _CefV8Context.Evaluate(() => iJSObject.GetValue("_globalId_").GetUIntValue());
        }

        public uint CreateAndGetID(CefV8Value iJSObject)
        {
            return _CefV8Context.Evaluate(() =>
            {
                var value = iJSObject.GetValue("_globalId_");
                if (value.IsUInt) return value.GetUIntValue();

                iJSObject.SetValue("_globalId_", CefV8Value.CreateUInt(++_Count), CefV8PropertyAttribute.DontDelete | CefV8PropertyAttribute.ReadOnly | CefV8PropertyAttribute.DontEnum);
                return _Count;
            });
        }

        public bool HasRelevantId(CefV8Value iJSObject)
        {
            if (iJSObject.IsUserCreated)
                return false;

            return _CefV8Context.EvaluateAsync(() =>
            {
                return iJSObject.HasValue("_globalId_");
            }).Result;
        }

    }
}
