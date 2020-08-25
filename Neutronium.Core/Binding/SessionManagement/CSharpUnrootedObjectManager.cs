using System;
using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Mapper;
using Neutronium.Core.Binding.Updater;

namespace Neutronium.Core.Binding.SessionManagement
{
    internal class CSharpUnrootedObjectManager: ICSharpUnrootedObjectManager
    {
        private readonly List<IJsCsGlue> _UnrootedEntities = new List<IJsCsGlue>();
        private readonly IJsUpdateHelper _JsUpdateHelper;
        private readonly ICSharpToGlueMapper _CSharpToGlueMapper;

        public CSharpUnrootedObjectManager(IJsUpdateHelper jsUpdateHelper, ICSharpToGlueMapper cSharpToGlueMapper)
        {
            _JsUpdateHelper = jsUpdateHelper;
            _CSharpToGlueMapper = cSharpToGlueMapper;
        }

        public void RegisterInSession(object newCSharpObject, Action<IJsCsGlue> performAfterBuild)
        {
            _JsUpdateHelper.CheckUiContext();

            var value = _CSharpToGlueMapper.Map(newCSharpObject);
            if (value == null)
                return;

            var updater = GetUnrootedEntitiesUpdater(value, performAfterBuild);
            _JsUpdateHelper.DispatchInJavascriptContext(() =>
            {
                _JsUpdateHelper.UpdateOnJavascriptContext(updater, value);
            });
        }

        private BridgeUpdater GetUnrootedEntitiesUpdater(IJsCsGlue newBridgeChild, Action<IJsCsGlue> performAfterBuild)
        {
            _UnrootedEntities.Add(newBridgeChild.AddRef());
            return new BridgeUpdater(updater =>
            {
                updater.InjectDetached(newBridgeChild.GetJsSessionValue());
                performAfterBuild(newBridgeChild);
            });
        }

        public void Dispose()
        {
            _UnrootedEntities.Clear();
        }
    }
}
