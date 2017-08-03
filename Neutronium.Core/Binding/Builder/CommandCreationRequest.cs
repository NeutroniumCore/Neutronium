using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Binding.Builder
{
    internal class CommandCreationRequest
    {
        internal IList<IJSCSGlue> CommandExecutableBuildingRequested { get; } = new List<IJSCSGlue>();
        internal IList<IJSCSGlue> CommandNotExecutableBuildingRequested { get; } = new List<IJSCSGlue>();

        public void AddRequest(IJSCSGlue commandGlue, bool canExecute)
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
