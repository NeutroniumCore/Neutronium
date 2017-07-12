using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chromium;
using Chromium.Remote;
using MoreCollection.Extensions;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.WebBrowserEngine.ChromiumFx.Convertion;
using Neutronium.WebBrowserEngine.ChromiumFx.V8Object;

namespace Neutronium.WebBrowserEngine.ChromiumFx.EngineBinding 
{
    internal class ChromiumFxFactory : IJavascriptObjectFactory 
    {
        private static uint _Count = 0;
        private readonly CfrV8Context _CfrV8Context;
        private static readonly IDictionary<Type, Func<object, CfrV8Value>> _Converters = new Dictionary<Type, Func<object, CfrV8Value>>();
        private readonly Lazy<CfrV8Value> _Factory;

        private readonly Lazy<CfrV8Value> _ObjectBuilder;
        private readonly Lazy<CfrV8Value> _ObjectBulkBuilder;
        private readonly Lazy<CfrV8Value> _ArrayBulkBuilder;
        private readonly Lazy<CfrV8Value> _ObjectCreationCallbackFunction;

        private readonly ChromiumFxObjectCreationCallBack _ObjectCallback = new ChromiumFxObjectCreationCallBack();

        private IJavascriptObject _Null;
        
        internal ChromiumFxFactory(CfrV8Context context) 
        {
            _CfrV8Context = context;
            _Factory = new Lazy<CfrV8Value>(FactoryCreator);
            _ObjectBuilder = new Lazy<CfrV8Value>(ObjectBuilderCreator);
            _ObjectBulkBuilder = new Lazy<CfrV8Value>(ObjectBulkBuilderCreator);
            _ArrayBulkBuilder = new Lazy<CfrV8Value>(ArrayBulkBuilderCreator);
            _ObjectCreationCallbackFunction = new Lazy<CfrV8Value>(ObjectCreationCallbackFunctionCreator);
        }

        static ChromiumFxFactory() 
        {
            Register<string>(CfrV8Value.CreateString);
            Register<Int64>((source) => CfrV8Value.CreateDouble((double) source));
            Register<UInt64>((source) => CfrV8Value.CreateDouble((double) source));
            Register<float>((source) => CfrV8Value.CreateDouble((double) source));
            Register<Int32>(CfrV8Value.CreateInt);
            Register<Int16>((source) => CfrV8Value.CreateInt((int) source));
            Register<UInt32>(CfrV8Value.CreateUint);
            Register<UInt16>((source) => CfrV8Value.CreateUint((UInt32) source));
            Register<char>((source) => CfrV8Value.CreateString(new StringBuilder().Append(source).ToString()));
            Register<double>(CfrV8Value.CreateDouble);
            Register<decimal>((source) => CfrV8Value.CreateDouble((double) source));
            Register<bool>(CfrV8Value.CreateBool);
            Register<DateTime>((source) => CfrV8Value.CreateDate(CfrTime.FromUniversalTime(source.ToUniversalTime())));
        }

        private CfrV8Value FactoryCreator()
        {
            var builderScript = @"(function(){
                const maxCount = 240000

                function pushResult(fn, array){
                    const count = array.length
                    if (count < maxCount) {
                        fn.apply(null, array)
                        return;
                    }
                    for(var index =0; index< count; index += maxCount) {
                        const subArray = array.slice(index, index+maxCount)
                        fn.apply(null, subArray)
                    }
                }

                function objectWithId(id){
                    this.{{ChromiumFXJavascriptRoot.IdName}} = id
                }
                function createObject(id){
                    return new objectWithId(id)
                }
                function createArray(id){
                    const res = []
                    res.{{ChromiumFXJavascriptRoot.IdName}} = id
                    return res
                }
                function createBulkObject(id, size, fn){
                    const array = []
                    for (var i = 0; i < size; i++) {
                        array.push(new objectWithId(id++))
                    }
                    pushResult(fn, array)
                }
                function createBulkArray(id, size, fn){
                    const array = []
                    for (var i = 0; i < size; i++) {
                        array.push(createArray(id++))
                    }
                    pushResult(fn, array)
                }
                return {
                    createObject,
                    createBulkObject,
                    createBulkArray
                };
            }())";

            var finalString = builderScript.Replace("{{ChromiumFXJavascriptRoot.IdName}}", ChromiumFXJavascriptRoot.IdName);
            return Eval(finalString);
        }

        private CfrV8Value GetProperty(string atttibute) => _Factory.Value.GetValue(atttibute);

        private CfrV8Value ObjectBuilderCreator() => GetProperty("createObject");
        private CfrV8Value ObjectBulkBuilderCreator() => GetProperty("createBulkObject");
        private CfrV8Value ArrayBulkBuilderCreator() => GetProperty("createBulkArray");

        private CfrV8Value ObjectCreationCallbackFunctionCreator()
        {
            return CfrV8Value.CreateFunction("objectCallBack", _ObjectCallback.Handler);
        }

        public static bool IsTypeConvertible(Type type) 
        {
            return type != null && _Converters.ContainsKey(type);
        }

        private static void Register<T>(Func<T, CfrV8Value> Factory) 
        {
            _Converters.Add(typeof(T), (o) => Factory((T) o));
        }

        public bool CreateBasic(object ifrom, out IJavascriptObject res) 
        {
            Func<object, CfrV8Value> conv;
            if (!_Converters.TryGetValue(ifrom.GetType(), out conv)) 
            {
                res = null;
                return false;
            }

            res = conv(ifrom).ConvertBasic();
            return true;
        }

        public IEnumerable<IJavascriptObject> CreateBasics(IReadOnlyList<object> from)
        {
            foreach (var @object in from)
            {
                IJavascriptObject res = null;
                yield return CreateBasic(@object, out res) ? res : null;
            }
        }

        public bool IsTypeBasic(Type type) 
        {
            return IsTypeConvertible(type);
        }

        public IJavascriptObject CreateNull() 
        {
            return _Null?? (_Null = CfrV8Value.CreateNull().ConvertBasic());
        }

        public IJavascriptObject CreateObject(bool local) 
        {
            var id = GetNextId();
            return _ObjectBuilder.Value.ExecuteFunction(null, new[] { CfrV8Value.CreateInt((int)id) }).ConvertObject(id);
        }

        public IJavascriptObject CreateObject(string creationCode) 
        {
            var v8Res = Eval(creationCode);
            return (v8Res!=null) ? UpdateConvert(v8Res) : null;
        }

        public IEnumerable<IJavascriptObject> CreateObjects(bool local, int number)
        {
            _ObjectBulkBuilder.Value.ExecuteFunction(null, new[] {
                CfrV8Value.CreateInt((int)_Count),
                CfrV8Value.CreateInt(number),
                _ObjectCreationCallbackFunction.Value
            });
            var results = _ObjectCallback.GetLastArguments();
            return results.Select(result => result.ConvertObject(_Count++));
        }

        public IJavascriptObject CreateUndefined() 
        {
            return CfrV8Value.CreateUndefined().ConvertBasic();
        }

        public IJavascriptObject CreateInt(int value) 
        {
            return CfrV8Value.CreateInt(value).ConvertBasic();
        }

        public IJavascriptObject CreateDouble(double value) 
        {
            return CfrV8Value.CreateDouble(value).ConvertBasic();
        }

        public IJavascriptObject CreateString(string value) 
        {
            return CfrV8Value.CreateString(value).ConvertBasic();
        }

        public IJavascriptObject CreateBool(bool value) 
        {
            return CfrV8Value.CreateBool(value).ConvertBasic();
        }

        public IJavascriptObject CreateArray(IEnumerable<IJavascriptObject> collection) 
        {
            var col = collection.ToList();
            var res = CfrV8Value.CreateArray(col.Count);
            col.ForEach((el, i) => res.SetValue(i, el.Convert()));
            return UpdateConvert(res, true);
        }

        public IEnumerable<IJavascriptObject> CreateArrays(int number)
        {
            _ArrayBulkBuilder.Value.ExecuteFunction(null, new[] {
                CfrV8Value.CreateInt((int)_Count),
                CfrV8Value.CreateInt(number),
                _ObjectCreationCallbackFunction.Value
            });
            var results = _ObjectCallback.GetLastArguments();
            return results.Select(result => result.ConvertBasic(_Count++));
        }

        public IJavascriptObject CreateArray(int size)
        {
            var res = CfrV8Value.CreateArray(size);
            return UpdateConvert(res, true);
        }

        private CfrV8Value Eval(string code)
        {
            CfrV8Value v8Res;
            CfrV8Exception exception;
            _CfrV8Context.Eval(code, string.Empty, 1, out v8Res, out exception);
            return v8Res;
        }

        private IJavascriptObject UpdateConvert(CfrV8Value value, bool isArray=false) 
        {
            if (value == null)
                return null;

            var id = GetNextId();
            value.SetValue(ChromiumFXJavascriptRoot.IdName, CfrV8Value.CreateUint(id), CfxV8PropertyAttribute.DontDelete  | CfxV8PropertyAttribute.DontEnum
                        |  CfxV8PropertyAttribute.ReadOnly);

            return isArray? value.ConvertBasic(id) : value.ConvertObject(id);
        }

        private uint GetNextId()
        {
            return _Count++;
        }
    }
}
