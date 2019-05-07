using System.Windows.Input;
using Neutronium.Core.Binding.GlueObject.Executable;
using Neutronium.Core.Binding.Listeners;

namespace Neutronium.Core.Binding.Updaters
{
    internal class CommandJavascriptUpdater : IJavascriptUpdater
    {
        private readonly object _Command;
        private readonly IJsUpdateHelper _JsUpdateHelper;
        private JsCommandBase _JsCommandBase;
        private byte _UpdateCount;

        public bool NeedToRunOnJsContext { get; private set; } = false;


        public CommandJavascriptUpdater(IJsUpdateHelper jsUpdateHelper, object command)
        {
            _Command = command;
            _JsUpdateHelper = jsUpdateHelper;

            if (_Command == null)
                return;

            _JsCommandBase = _JsUpdateHelper.GetCached<JsCommandBase>(_Command);

            if (_JsCommandBase == null)
                return;

            _UpdateCount = _JsCommandBase.NextUpdateCount;
        }

        public void OnUiContext(ObjectChangesListener off)
        {
            if (_JsCommandBase == null)
                return;

            var count = _JsCommandBase.CurrentUpdateCount;
            if (count == _UpdateCount)
                return;

            NeedToRunOnJsContext = true;
            _JsCommandBase.UpdateCount(_UpdateCount);
        }      

        public void OnJsContext()
        {
            _JsCommandBase.SetUpdateCountOnJsContext();
        }
    }
}
