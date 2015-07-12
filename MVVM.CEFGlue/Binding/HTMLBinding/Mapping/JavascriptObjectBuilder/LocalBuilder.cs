using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xilium.CefGlue;

using MVVM.CEFGlue.Infra;
using MVVM.CEFGlue.Exceptions;
using MVVM.CEFGlue.CefGlueHelper;

namespace MVVM.CEFGlue.HTMLBinding
{
    public class LocalBuilder : IJSOLocalBuilder
    {
       private static uint _MapCount = 0;
       private CefV8Context _CefV8Context;

       public LocalBuilder(CefV8Context iIWebView)
       {
           _CefV8Context = iIWebView;
       }

       private CefV8Value UnsafeCreateJSO()
        {
            _CefV8Context.Enter();
            //JSObject res =new JSObject();
            CefV8Value res = CefV8Value.CreateObject(null);
            res.SetValue("_MappedId", CefV8Value.CreateUInt(_MapCount++), 
                CefV8PropertyAttribute.ReadOnly |CefV8PropertyAttribute.DontEnum |CefV8PropertyAttribute.DontDelete );
            //res["_MappedId"] = new JSValue(_MapCount++);
            _CefV8Context.Exit();
            return res;
        }



        public CefV8Value CreateJSO()
        {
            return _CefV8Context.EvaluateAsync(() => UnsafeCreateJSO()).Result;
        }

        public uint GetID(CefV8Value iJSObject)
        {
            if (iJSObject == null)
                return 0;

            return _CefV8Context.EvaluateAsync(() => iJSObject.GetValue("_MappedId").GetUIntValue()).Result;
        }


        public bool HasRelevantId(CefV8Value iJSObject)
        {
            return ((iJSObject!=null) &&( iJSObject.HasValue("_MappedId")));
        }

        public CefV8Value CreateDate(DateTime dt)
        {
            return CefV8Value.CreateDate(dt);
        }

        private CefV8Value UpdateObject(CefV8Value ires)
        {
            if (ires != null)
            {
                ires.SetValue("_MappedId", CefV8Value.CreateUInt(_MapCount++),
                    CefV8PropertyAttribute.ReadOnly | CefV8PropertyAttribute.DontEnum | CefV8PropertyAttribute.DontDelete);
            }

            return ires;
        }


        private CefV8Value Check(CefV8Value ires)
        {
            if (ires == null)
                throw ExceptionHelper.NoKoExtension();

            return ires;
        }

        private CefV8Value CheckUpdate(CefV8Value ires)
        {
            return UpdateObject(Check(ires));
        }



        public CefV8Value CreateEnum(Enum ienum)
        {
            return _CefV8Context.EvaluateAsync(() =>
                {
                    CefV8Value res = null;
                    CefV8Exception excep = null;
                    _CefV8Context.Enter();
                    _CefV8Context.TryEval(string.Format("new Enum('{0}',{1},'{2}','{3}')",
                                ienum.GetType().Name, Convert.ToInt32(ienum), ienum.ToString(), ienum.GetDescription()),
                                out res, out excep);

                    _CefV8Context.Exit();
                   
                    return CheckUpdate(res);
                }).Result;
        }

        //private CefV8Value? _JSNull = null;

        public CefV8Value CreateNull()
        {
            return _CefV8Context.EvaluateAsync(() =>
            {
                _CefV8Context.Enter();
                CefV8Value myres = CefV8Value.CreateNull();
                _CefV8Context.Exit();
                return myres;

            }).Result;


            //return CefV8Value.CreateNull();
            //if (!_JSNull.HasValue)
            //    _JSNull = Check(_IWebView.EvaluateSafe(() => _IWebView.ExecuteJavascriptWithResult("new Null_reference()")));

            //return _JSNull.Value;
        }
    }
}
