using Neutronium.Core.Binding.GlueObject;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.Builder
{
    internal class CommandCreationRequest
    {
        private List<IJsCsGlue> _Executables;
        private List<IJsCsGlue> _NotExecutables;

        internal IList<IJsCsGlue> CommandExecutableBuildingRequested => _Executables;
        internal IList<IJsCsGlue> CommandNotExecutableBuildingRequested => _NotExecutables;

        private List<IJsCsGlue> ExecutablesForAdd => _Executables ?? (_Executables = new List<IJsCsGlue>());
        private List<IJsCsGlue> NotExecutablesForAdd => _NotExecutables ?? (_NotExecutables = new List<IJsCsGlue>());

        public void AddRequest(IJsCsGlue commandGlue, bool canExecute)
        {
            var list = GetForAdd(canExecute);
            list.Add(commandGlue);
        }

        private List<IJsCsGlue> GetForAdd(bool canExecute)
        {
            return canExecute ? ExecutablesForAdd : NotExecutablesForAdd;
        }
    }
}
