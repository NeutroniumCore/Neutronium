using Neutronium.Core.Binding.GlueObject.Executable;
using Neutronium.Core.Binding.Listeners;

namespace Neutronium.Core.Binding.Updaters
{
    internal class CommandJavascriptUiContextUpdater : IJavascriptUIContextUpdater
    {
        private readonly object _Command;
        private readonly IJsUpdateHelper _JsUpdateHelper;
        private readonly JsCommandBase _JsCommandBase;
        private readonly byte _UpdateCount;

        public CommandJavascriptUiContextUpdater(IJsUpdateHelper jsUpdateHelper, object command)
        {
            _Command = command;
            _JsUpdateHelper = jsUpdateHelper;

            if (_Command == null)
                return;

            _JsCommandBase = _JsUpdateHelper.GetCached<JsCommandBase>(_Command);
            _UpdateCount = _JsCommandBase?.NextUpdateCount ?? 0;
        }

        public IJavascriptJsContextUpdater ExecuteOnUiContext(ObjectChangesListener off)
        {
            if (_JsCommandBase == null)
                return null;

            var count = _JsCommandBase.CurrentUpdateCount;
            if (count == _UpdateCount)
                return null;

            _JsCommandBase.UpdateCount(_UpdateCount);
            return new CommandJsContextUpdater(_JsCommandBase);
        }

        private class CommandJsContextUpdater : IJavascriptJsContextUpdater
        {
            private readonly JsCommandBase _JsCommandBase;

            public CommandJsContextUpdater(JsCommandBase jsCommandBase)
            {
                _JsCommandBase = jsCommandBase;
            }

            public void ExecuteOnJsContext()
            {
                _JsCommandBase.SetUpdateCountOnJsContext();
            }
        }
    }
}
