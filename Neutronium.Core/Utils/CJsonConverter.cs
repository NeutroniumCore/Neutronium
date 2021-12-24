using Neutronium.Core.Binding.GlueBuilder;
using Neutronium.Core.Binding.Mapper;
using Neutronium.Core.Binding.SessionManagement;
using Neutronium.Core.Extension;
using Neutronium.Core.Infra.VM;
using Neutronium.Core.Log;

namespace Neutronium.Core.Utils {
    /// <summary>
    /// Helper class to export C# object into cjson format (circular json).
    /// This format is compatible with neutronium-vue client: https://github.com/NeutroniumCore/neutronium-vue
    /// using the neutronium-vm-loader: https://github.com/NeutroniumCore/neutronium-vm-loader
    /// </summary>
    public class CJsonConverter
    {
        private readonly CSharpToGlueMapper _Converter;

        /// <summary>
        /// Instanciate a new CJsonConverter
        /// </summary>
        public CJsonConverter()
        {
            var cache = new SessionCacher();
            var factory = new GlueFactory(null, cache, null, null);
            _Converter = new CSharpToGlueMapper(cache, factory, new NullLogger());
        }

        /// <summary>
        /// raw conversion to cjson format
        /// </summary>
        /// <param name="object">object to be serialiazed</param>
        /// <returns>cjson string value</returns>
        public string ToCjson(object @object)
        {
            var glue = _Converter.Map(@object);
            return glue.AsCircularJson();
        }

        /// <summary>
        /// Conversion to cjson format compatible with a data context root Vm
        /// Can be used as an entry for neutronium-vm-loader https://github.com/NeutroniumCore/neutronium-vm-loader
        /// </summary>
        /// <param name="object">object to be serialiazed</param>
        /// <returns>cjson string value</returns>
        public string ToRootVmCjson(object @object)
        {
            var context = new DataContextViewModel(@object);
            var glue = _Converter.Map(context);
            return glue.AsCircularVersionedJson();
        }
    }
}
