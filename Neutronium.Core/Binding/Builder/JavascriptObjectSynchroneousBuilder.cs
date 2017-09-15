using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Builder 
{
    internal class JavascriptObjectSynchroneousBuilder
    {
        private readonly IJavascriptObjectFactory _Factory;
        private readonly IJavascriptSessionCache _Cache;
        private readonly IJsCsGlue _Root;
        private readonly bool _Mapping;

        public JavascriptObjectSynchroneousBuilder(IJavascriptObjectFactory factory, IJavascriptSessionCache cache, IJsCsGlue root, bool mapping)
        {
            _Mapping = mapping;
            _Factory = factory;
            _Cache = cache;
            _Root = root;
        }

        public void UpdateJavascriptValue()
        {
            VisitUpdate(_Root);
        }

        private void VisitUpdate(IJsCsGlue glue)
        {
            if (glue.JsValue != null)
                return;

            var updater = new JavascriptObjectSynchroneousBuilderAdapter(_Factory, _Cache, glue, _Mapping);
            updater.ApplyLocalChanges();
            glue.VisitChildren(VisitUpdate);
            updater.AfterChildrenUpdates();
        }
    }
}
