using System.Collections.Generic;
using System.Text;

namespace Neutronium.Core.Binding.GlueObject
{
    public class NameContext
    {
        private readonly HashSet<IJSCSGlue> _AlreadyComputed = new HashSet<IJSCSGlue>();
        private readonly StringBuilder _NameBuilder = new StringBuilder();

        public bool AlreadyComputed(IJSCSGlue glue)
        {
            return _AlreadyComputed.Add(glue);
        }

        public void Append(string value)
        {
            _NameBuilder.Append(value);
        }

        public string BuildString()
        {
            return _NameBuilder.ToString();
        }
    }
}
