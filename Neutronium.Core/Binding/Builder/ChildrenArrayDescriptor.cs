using Neutronium.Core.Binding.GlueObject;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Binding.Builder
{
    public class ChildrenArrayDescriptor : ChildrenDescriptor<int>
    {
        public ChildrenArrayDescriptor(IJSCSGlue father, IList<IJSCSGlue> description) :
            base(father, description?.Select((d, index) => new ChildDescription<int>(index, d)))
        {
        }
    }
}
