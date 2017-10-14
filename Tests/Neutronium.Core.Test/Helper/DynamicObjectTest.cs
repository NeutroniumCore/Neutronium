using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;

namespace Neutronium.Core.Test.Helper
{
    public class DynamicObjectTest : DynamicObject
    {
        private readonly Dictionary<string, int> _Dictionary = new Dictionary<string, int>
        {
            ["string"] = 6,
            ["int"] = 3,
            ["size"] = 4
        };

        public override IEnumerable<string> GetDynamicMemberNames() => _Dictionary.Keys;

        public int classic => 23;

        [Bindable(false, BindingDirection.OneWay)]

        public string Invisible { get; set; }

        public string ReadOnlyByNature { get; private set; }

        [Bindable(true, BindingDirection.OneWay)]
        public string ReadOnlyByAttribute { get; set; }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            int res;
            if (_Dictionary.TryGetValue(binder.Name, out res))
            {
                result = res;
                return true;
            }
            result = null;
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _Dictionary[binder.Name] = (int)value;
            return true;
        }
    }
}
