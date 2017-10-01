using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Example.Dictionary.Cfx.Vue.ViewModel
{
    public class DynamicObjectViewModel : DynamicObject
    {
        public string Information => "I am a DynamicObject";

        public override IEnumerable<string> GetDynamicMemberNames() => Enumerable.Range(0, 10).Select(r => $"Property{r}");

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
    }
}
