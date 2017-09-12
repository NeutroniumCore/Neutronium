using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chromium;
using Chromium.Remote;
using MoreCollection.Extensions;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.WebBrowserEngine.ChromiumFx.Convertion;

namespace Neutronium.WebBrowserEngine.ChromiumFx.EngineBinding
{
    internal class ChromiumFxFactory : IJavascriptObjectFactory
    {
        private static uint _Count = 1;
        private readonly CfrV8Context _CfrV8Context;
        private readonly IWebView _WebView;
        private static readonly IDictionary<Type, Func<object, CfrV8Value>> _Converters = new Dictionary<Type, Func<object, CfrV8Value>>();
        private readonly Lazy<CfrV8Value> _Factory;

        private readonly Lazy<CfrV8Value> _ObjectBuilder;
        private readonly Lazy<CfrV8Value> _ObjectBulkBuilder;
        private readonly Lazy<CfrV8Value> _ArrayBulkBuilder;
        private readonly Lazy<CfrV8Value> _BasicBulkBuilder;
        private readonly Lazy<CfrV8Value> _ObjectWithConstructorBulkBuilder;
        private readonly Lazy<CfrV8Value> _ObjectCreationCallbackFunction;
        
        private readonly ChromiumFxObjectCreationCallBack _ObjectCallback = new ChromiumFxObjectCreationCallBack();

        private IJavascriptObject _Null;

        internal ChromiumFxFactory(CfrV8Context context, IWebView webView)
        {
            _WebView = webView;
            _CfrV8Context = context;
            _Factory = new Lazy<CfrV8Value>(FactoryCreator);
            _ObjectBuilder = new Lazy<CfrV8Value>(ObjectBuilderCreator);
            _ObjectBulkBuilder = new Lazy<CfrV8Value>(ObjectBulkBuilderCreator);
            _ArrayBulkBuilder = new Lazy<CfrV8Value>(ArrayBulkBuilderCreator);
            _BasicBulkBuilder = new Lazy<CfrV8Value>(BasicBulkBuilderCreator);
            _ObjectWithConstructorBulkBuilder = new Lazy<CfrV8Value>(ObjectWithConstructorBulkBuilderCreator);
            
            _ObjectCreationCallbackFunction = new Lazy<CfrV8Value>(ObjectCreationCallbackFunctionCreator);
        }

        static ChromiumFxFactory()
        {
            Register<string>(CfrV8Value.CreateString);
            Register<Int64>((source) => CfrV8Value.CreateDouble((double)source));
            Register<UInt64>((source) => CfrV8Value.CreateDouble((double)source));
            Register<float>((source) => CfrV8Value.CreateDouble((double)source));
            Register<Int32>(CfrV8Value.CreateInt);
            Register<Int16>((source) => CfrV8Value.CreateInt((int)source));
            Register<UInt32>(CfrV8Value.CreateUint);
            Register<UInt16>((source) => CfrV8Value.CreateUint((UInt32)source));
            Register<char>((source) => CfrV8Value.CreateString(new StringBuilder().Append(source).ToString()));
            Register<double>(CfrV8Value.CreateDouble);
            Register<decimal>((source) => CfrV8Value.CreateDouble((double)source));
            Register<bool>(CfrV8Value.CreateBool);
            Register<DateTime>((source) => CfrV8Value.CreateDate(CfrTime.FromUniversalTime(source.ToUniversalTime())));
        }

        private static void Register<T>(Func<T, CfrV8Value> Factory)
        {
            _Converters.Add(typeof(T), (o) => Factory((T)o));
        }

        private CfrV8Value FactoryCreator()
        {
            var builderScript = @"(function(){
                const maxCount = {{MaxFunctionArgumentsNumber}}

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
                function objectWithId(id, readOnly){
                    Object.defineProperty(this, '{{NeutroniumConstants.ObjectId}}', {value: id});
                    Object.defineProperty(this, '{{NeutroniumConstants.ReadOnlyFlag}}', {value: readOnly});
                }
                function createObject(id, readOnly){
                    return new objectWithId(id, readOnly)
                }
                function createArray(id){
                    const res = []
                    Object.defineProperty(res, '{{NeutroniumConstants.ObjectId}}', {value: id});
                    return res
                }
                function pushObjects(array, id, number, readOnly){
                    for (var i = 0; i < number; i++) {
                        array.push(new objectWithId(id++, readOnly))
                    }
                    return id;
                }
                function createBulkObject(id, readWriteNumber, readOnlyNumber, fn){
                    const array = []
                    id = pushObjects(array, id, readWriteNumber, false)
                    pushObjects(array, id, readOnlyNumber, true)
                    pushResult(fn, array)
                }
                function createBulkBasic(values, fn){
                    const array = eval(values)
                    pushResult(fn, array)
                }
                function createBulkArray(id, size, fn){
                    const array = []
                    for (var i = 0; i < size; i++) {
                        array.push(createArray(id++))
                    }
                    pushResult(fn, array)
                }
                function createBulkObjectWithConstructor(id, number, constructor,  fn){
                    const array = []
                    const allArgs = Array.from(arguments)
                    const args = allArgs.slice(4)
                    for (var i = 0; i < number; i++) {
                        array.push(new constructor(id++, ...args))
                    }
                    pushResult(fn, array)
                }
                return {
                    createObject,
                    createBulkObject,
                    createBulkArray,
                    createBulkBasic,
                    createBulkObjectWithConstructor
                };
            }())";

            var finalString = builderScript.Replace("{{MaxFunctionArgumentsNumber}}", $"{_WebView.MaxFunctionArgumentsNumber}")
                                           .Replace("{{NeutroniumConstants.ObjectId}}", NeutroniumConstants.ObjectId)
                                           .Replace("{{NeutroniumConstants.ReadOnlyFlag}}", NeutroniumConstants.ReadOnlyFlag);
            return Eval(finalString);
        }

        private CfrV8Value GetProperty(string atttibute) => _Factory.Value.GetValue(atttibute);

        private CfrV8Value ObjectBuilderCreator() => GetProperty("createObject");
        private CfrV8Value ObjectBulkBuilderCreator() => GetProperty("createBulkObject");
        private CfrV8Value ArrayBulkBuilderCreator() => GetProperty("createBulkArray");
        private CfrV8Value BasicBulkBuilderCreator() => GetProperty("createBulkBasic");
        private CfrV8Value ObjectWithConstructorBulkBuilderCreator() => GetProperty("createBulkObjectWithConstructor");

        private CfrV8Value ObjectCreationCallbackFunctionCreator()
        {
            return CfrV8Value.CreateFunction("objectCallBack", _ObjectCallback.Handler);
        }

        public static bool IsTypeConvertible(Type type)
        {
            return type != null && _Converters.ContainsKey(type);
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

        public IEnumerable<IJavascriptObject> CreateBasics(IEnumerable<object> from)
        {
            _BasicBulkBuilder.Value.ExecuteFunction(null, new[] {
                CfrV8Value.CreateString(CreateStringValue(from)),
                _ObjectCreationCallbackFunction.Value
            });
            return _ObjectCallback.GetLastArguments().Select(jso => jso.ConvertBasic());
        }

        private string CreateStringValue(IEnumerable<object> from)
        {
            return $"[{string.Join(",", from.Select(v => JavascriptNamer.GetCreateExpression(v)))}]";
        }

        public bool IsTypeBasic(Type type) 
        {
            return IsTypeConvertible(type);
        }

        public IJavascriptObject CreateNull() 
        {
            return _Null?? (_Null = CfrV8Value.CreateNull().ConvertBasic());
        }

        public IJavascriptObject CreateObject()
        {
            return CfrV8Value.CreateObject(null, null).ConvertObject();
        }

        public IJavascriptObject CreateObject(bool readOnly) 
        {
            var id = GetNextId();
            return _ObjectBuilder.Value.ExecuteFunction(null, new[] { CfrV8Value.CreateInt((int)id), CfrV8Value.CreateBool(readOnly) }).ConvertObject(id);
        }

        public IJavascriptObject CreateObject(string creationCode) 
        {
            var v8Res = Eval(creationCode);
            return (v8Res!=null) ? UpdateConvert(v8Res) : null;
        }

        public IEnumerable<IJavascriptObject> CreateObjects(int readWriteNumber, int readOnlyNumber)
        {
            _ObjectBulkBuilder.Value.ExecuteFunction(null, new[] {
                CfrV8Value.CreateInt((int)_Count),
                CfrV8Value.CreateInt(readWriteNumber),
                CfrV8Value.CreateInt(readOnlyNumber),
                _ObjectCreationCallbackFunction.Value
            });
            return _ObjectCallback.GetLastArguments().Select(js => js.ConvertObject(_Count++));
        }

        public IEnumerable<IJavascriptObject> CreateObjectsFromContructor(int number, IJavascriptObject constructor, params IJavascriptObject[] parameters)
        {
            var args = new List<CfrV8Value> {
                CfrV8Value.CreateInt((int)_Count),
                CfrV8Value.CreateInt(number),
                constructor.Convert(),
                _ObjectCreationCallbackFunction.Value
            };
            args.AddRange(parameters.Convert());

            _ObjectWithConstructorBulkBuilder.Value.ExecuteFunction(null, args.ToArray());
            return _ObjectCallback.GetLastArguments().Select(js => js.ConvertObject(_Count++));
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
            return _ObjectCallback.GetLastArguments().Select(js => js.ConvertBasic(_Count++));
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
            value.SetValue(NeutroniumConstants.ObjectId, CfrV8Value.CreateUint(id), CfxV8PropertyAttribute.DontDelete  | CfxV8PropertyAttribute.DontEnum
                        |  CfxV8PropertyAttribute.ReadOnly);

            return isArray? value.ConvertBasic(id) : value.ConvertObject(id);
        }

        private uint GetNextId() => _Count++;
    }
}
