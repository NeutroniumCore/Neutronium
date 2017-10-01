using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Neutronium.Core.Test.Helper
{
    public class TestDynamicObject : DynamicObject
    {
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var rgx = new Regex(@"^Property([0-9]+)$", RegexOptions.IgnoreCase);
            var res = rgx.Match(binder.Name);
            if (res.Success)
            {
                result = int.Parse(res.Groups[1].Value);
                return true;
            }
            result = null;
            return false;
        }

        public override IEnumerable<string> GetDynamicMemberNames() => Enumerable.Range(0, 10).Select(r => $"Property{r}");
    }
}
