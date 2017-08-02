using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;
using System.Linq;

namespace Neutronium.Core.Binding.Builder
{
    internal class CommandCreationRequest
    {
        public int CanExecuteNumber { get; private set; }
        public int CanNotExecuteNumber => _CommandBuildingRequested.Count - CanExecuteNumber;

        //private readonly List<IJSCSGlue> _CommandExecutableBuildingRequested = new List<IJSCSGlue>();
        //private readonly List<IJSCSGlue> _CommandNotExecutableBuildingRequested = new List<IJSCSGlue>();

        private readonly LinkedList<IJSCSGlue> _CommandBuildingRequested = new LinkedList<IJSCSGlue>();

        public void AddRequest(IJSCSGlue commandGlue, bool canExecute)
        {
            if (!canExecute)
            {
                _CommandBuildingRequested.AddLast(commandGlue);
                return;
            }

            _CommandBuildingRequested.AddFirst(commandGlue);
            CanExecuteNumber += 1;
        }

        internal IEnumerable<IJSCSGlue> GetElements() => _CommandBuildingRequested;
    }
}
