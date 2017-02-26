using MoreCollection.Extensions;
using Neutronium.Core.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutronium.Core.Binding.GlueObject
{
    public class DescriptionBuilder
    {
        private readonly Dictionary<IJSCSGlue,string> _AlreadyComputed = new Dictionary<IJSCSGlue, string>();
        private readonly Stack<string> _Context = new Stack<string>();
        private readonly StringBuilder _NameBuilder = new StringBuilder();

        public string GetContextualName(IJSCSGlue glue)
        {
            var found = _AlreadyComputed.GetOrAdd(glue, _ => $"\"~{(string.Join("~", _Context.Reverse()))}\"");
            return (found.CollectionStatus == CollectionStatus.Found) ? found.Item : null;
        }

        public void Append(string value)
        {
            _NameBuilder.Append(value);
        }

        public string BuildString()
        {
            return _NameBuilder.ToString();
        }

        public IDisposable PushContext(string context)
        {
            _Context.Push(context);
            return new DisposableAction(() => _Context.Pop());
        }

        public IDisposable PushContext(int context)
        {
            return PushContext(context.ToString());
        }
    }
}
