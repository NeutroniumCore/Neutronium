using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Updater;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Mapper
{
    internal struct RawGlueMapable : IGlueMapable
    {
        private readonly IJavascriptObject _JavascriptObject;
        public object Source { get; }

        internal RawGlueMapable(object source, IJavascriptObject javascriptObject)
        {
            Source = source;
            _JavascriptObject = javascriptObject;
        }

        
        public IJsCsGlue Map(IJsUpdateHelper helper)
        {
            return helper.MapJavascripObject(Source, _JavascriptObject);
        }
    }
}
