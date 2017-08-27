using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Binding.Builder
{
    internal class CommandCreationRequest
    {
        internal IList<IJsCsGlue> CommandExecutableBuildingRequested { get; } = new List<IJsCsGlue>();
        internal IList<IJsCsGlue> CommandNotExecutableBuildingRequested { get; } = new List<IJsCsGlue>();

        public void AddRequest(IJsCsGlue commandGlue, bool canExecute)
        {
            if (canExecute)
            {
                CommandExecutableBuildingRequested.Add(commandGlue);
            }
            else
            {
                CommandNotExecutableBuildingRequested.Add(commandGlue);
            }
        }
    }
}
