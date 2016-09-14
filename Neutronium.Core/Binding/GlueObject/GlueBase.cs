using System.Collections.Generic;
using System.Text;

namespace Neutronium.Core.Binding.GlueObject
{
    public abstract class GlueBase
    {
        protected abstract void ComputeString(StringBuilder sb, HashSet<IJSCSGlue> alreadyComputed);

        public void BuilString(StringBuilder sb, HashSet<IJSCSGlue> alreadyComputed)
        {
            if (!alreadyComputed.Add(this as IJSCSGlue))
                return;

            ComputeString(sb, alreadyComputed);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            BuilString(sb, new HashSet<IJSCSGlue>());
            return sb.ToString();
        }
    }
}
