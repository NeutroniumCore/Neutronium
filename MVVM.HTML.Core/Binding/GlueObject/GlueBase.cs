using System.Collections.Generic;
using System.Text;

namespace MVVM.HTML.Core.HTMLBinding
{
    public abstract class GlueBase
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            BuilString(sb, new HashSet<IJSCSGlue>());
            return sb.ToString();
        }

        protected abstract void ComputeString(StringBuilder sb, HashSet<IJSCSGlue> alreadyComputed);

        public void BuilString(StringBuilder sb, HashSet<IJSCSGlue> alreadyComputed)
        {
            if (!alreadyComputed.Add(this as IJSCSGlue))
                return;

            ComputeString(sb, alreadyComputed);
        }
    }
}
