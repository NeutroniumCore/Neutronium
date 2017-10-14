using System.Collections.Generic;
using System.Dynamic;

namespace Neutronium.Core.Test.Helper
{
    public class AmbigeousDynamicObject : DynamicObject
    {
        public override IEnumerable<string> GetDynamicMemberNames() => new[] { "Ambigeous" };

        public string Ambigeous => "static";

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = "dynamic";
            return true;
        }
    }
}
