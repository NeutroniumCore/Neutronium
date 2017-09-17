using System;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Listeners;

namespace Neutronium.Core.Binding.GlueBuilder 
{
    internal class GlueFactoryBase 
    {
        public event EventHandler<IJsCsGlue> ElementCreated;
        private readonly ICSharpToJsCache _Cacher;
        private readonly ObjectChangesListener _OnListener;

        public GlueFactoryBase(ICSharpToJsCache cacher, ObjectChangesListener onListener)
        {
            _Cacher = cacher;
            _OnListener = onListener;
        }

        public JsBasicObject BuildBasic(object basic) 
        {
            return CacheWithoutListen(basic, new JsBasicObject(basic));
        }

        public JsInt BuildInt(int value)
        {
            return CacheWithoutListen(value, new JsInt(value));
        }

        public JsString BuildString(string value)
        {
            return CacheWithoutListen(value, new JsString(value));
        }

        private T CacheWithoutListen<T>(object key, T glue) where T : IJsCsGlue 
        {
            if (key != null)
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
