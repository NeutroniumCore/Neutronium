using MoreCollection.Extensions;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.Builder
{
    internal class JavascriptObjectSynchroneousBuilder
    {
        private readonly IJavascriptObjectFactory _Factory;
        private readonly IJavascriptSessionCache _Cache;
        private readonly IJSCSGlue _Root;

        public JavascriptObjectSynchroneousBuilder(IJavascriptObjectFactory factory, IJavascriptSessionCache cache, IJSCSGlue root)
        {
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

            var updater = new JavascriptObjectSynchroneousBuilderAdapter(_Factory, _Cache, glue);
            updater.ApplyLocalChanges();
            glue.GetChildren().ForEach(VisitUpdate);
            updater.AfterChildrenUpdates();
        }
    }
}
