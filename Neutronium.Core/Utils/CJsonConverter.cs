using Neutronium.Core.Binding;
using Neutronium.Core.Binding.GlueBuilder;
using Neutronium.Core.Extension;
using Neutronium.Core.Infra.VM;
using Neutronium.Core.Log;

namespace Neutronium.Core.Utils
{
    public class CJsonConverter
    {
        private readonly CSharpToJavascriptConverter _Converter;

        public CJsonConverter()
        {
            var cache = new SessionCacher();
            var factory = new GlueFactory(null, cache, null, null);
            _Converter = new CSharpToJavascriptConverter(cache, factory, new NullLogger());
        }

        public string ToCjson(object @object)
        {
            var glue = _Converter.Map(@object);
            return glue.AsCircularJson();
        }

        public string ToRootVmCjson(object @object)
        {
            var context = new DataContextViewModel(@object);
            var glue = _Converter.Map(context);
            return glue.AsCircularVersionedJson();
        }
    }
}
