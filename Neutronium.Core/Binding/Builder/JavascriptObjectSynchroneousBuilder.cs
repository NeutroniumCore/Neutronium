using MoreCollection.Extensions;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Builder
{
    internal class JavascriptObjectSynchroneousBuilder
    {
        private readonly IJavascriptObjectFactory _Factory;
        private readonly IJavascriptSessionCache _Cache;
        private readonly IJSCSGlue _Root;
        private readonly bool _Mapping;

        public JavascriptObjectSynchroneousBuilder(IJavascriptObjectFactory factory, IJavascriptSessionCache cache, IJSCSGlue root, bool mapping)
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

        private void VisitUpdate(IJSCSGlue glue)
        {
            if (glue.JSValue != null)
                return;

            var updater = new JavascriptObjectSynchroneousBuilderAdapter(_Factory, _Cache, glue, _Mapping);
            updater.ApplyLocalChanges();
            glue.GetChildren()?.ForEach(VisitUpdate);
            updater.AfterChildrenUpdates();
        }
    }
}
