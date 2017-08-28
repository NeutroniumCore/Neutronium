using System;

namespace Neutronium.Core.Binding.GlueObject.Factory 
{
    internal class GlueFactoryBase 
    {
        public event EventHandler<IJsCsGlue> ElementCreated;
        private readonly ICSharpToJsCache _Cacher; 

        public GlueFactoryBase(ICSharpToJsCache cacher)
        {
            _Cacher = cacher;
        }

        public JsBasicObject BuildBasic(object basic) 
        {
            return Cache(basic, new JsBasicObject(basic));
        }

        protected T Cache<T>(object key, T glue) where T : IJsCsGlue 
        {
            if (key != null)
                _Cacher.CacheFromCSharpValue(key, glue);

            ElementCreated?.Invoke(this, glue);
            return glue;
        }
    }
}
