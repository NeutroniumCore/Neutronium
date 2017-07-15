using Neutronium.Core.Binding.GlueObject;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Binding.Builder
{
    public class ChildrenPropertyDescriptor : ChildrenDescriptor<string>
    {
        public ChildrenPropertyDescriptor(IJSCSGlue father, IReadOnlyDictionary<string, IJSCSGlue> description) :
            base(father, description?.Select(d => new ChildDescription<string>(d.Key, d.Value)))
        {
        }
    }
}
