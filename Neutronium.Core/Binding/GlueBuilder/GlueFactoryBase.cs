using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.GlueObject.Basic;
using Neutronium.Core.Binding.Listeners;
using System;

namespace Neutronium.Core.Binding.GlueBuilder
{
    internal abstract class GlueFactoryBase
    {
        public event EventHandler<IJsCsGlue> ElementCreated;
        private readonly ICSharpToJsCache _Cacher;
        private readonly ObjectChangesListener _OnListener;

        protected GlueFactoryBase(ICSharpToJsCache cacher, ObjectChangesListener onListener)
        {
            _Cacher = cacher;
            _OnListener = onListener;
        }

        public JsInt BuildInt(object value) => CacheWithoutListen(value, new JsInt((int)value));
        public JsByte BuildByte(object value) => CacheWithoutListen(value, new JsByte((byte)value));
        public JsSByte BuildSByte(object value) => CacheWithoutListen(value, new JsSByte((sbyte)value));
        public JsString BuildString(object value) => CacheWithoutListen(value, new JsString((string)value));
        public JsBool BuildBool(object value) => CacheWithoutListen(value, new JsBool((bool)value));
        public JsEnum BuildEnum(object value) => CacheWithoutListen(value, new JsEnum((Enum)value));
        public JsDouble BuildDouble(object value) => CacheWithoutListen(value, new JsDouble((double)value));
        public JsDecimal BuildDecimal(object value) => CacheWithoutListen(value, new JsDecimal((decimal)value));
        public JsUint BuildUint(object value) => CacheWithoutListen(value, new JsUint((uint)value));
        public JsLong BuildLong(object value) => CacheWithoutListen(value, new JsLong((long)value));
        public JsShort BuildShort(object value) => CacheWithoutListen(value, new JsShort((short)value));
        public JsFloat BuildFloat(object value) => CacheWithoutListen(value, new JsFloat((float)value));
        public JsUlong BuildUlong(object value) => CacheWithoutListen(value, new JsUlong((ulong)value));
        public JsUshort BuildUshort(object value) => CacheWithoutListen(value, new JsUshort((ushort)value));
        public JsDateTime BuildDateTime(object value) => CacheWithoutListen(value, new JsDateTime((DateTime)value));
        public JsChar BuildChar(object value) => CacheWithoutListen(value, new JsChar((char)value));

        private T CacheWithoutListen<T>(object key, T glue) where T : IJsCsGlue
        {
            _Cacher.CacheFromCSharpValue(key, glue);
            ElementCreated?.Invoke(this, glue);
            return glue;
        }

        protected T Cache<T>(object key, T glue) where T : IJsCsGlue
        {
            var result = CacheWithoutListen<T>(key, glue);
            if (_OnListener != null)
                result.ApplyOnListenable(_OnListener);
            return result;
        }
    }
}
