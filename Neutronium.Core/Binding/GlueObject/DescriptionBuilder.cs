using MoreCollection.Extensions;
using Neutronium.Core.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutronium.Core.Binding.GlueObject
{
    public class DescriptionBuilder: IDescriptionBuilder
    {
        private readonly Dictionary<IJsCsGlue,string> _AlreadyComputed = new Dictionary<IJsCsGlue, string>();
        private readonly Stack<string> _Context = new Stack<string>();
        private readonly StringBuilder _NameBuilder = new StringBuilder();
        private readonly string _CommandDescription;

        public int StringLength => _NameBuilder.Length;

        public DescriptionBuilder(string commandDescription = "{{}}")
        {
            _CommandDescription = commandDescription;
        }

        public string GetContextualName(IJsCsGlue glue)
        {
            var found = _AlreadyComputed.GetOrAdd(glue, _ => $"\"~{(string.Join("~", _Context.Reverse()))}\"");
            return (found.CollectionStatus == CollectionStatus.Found) ? found.Item : null;
        }

        public void Append(string value)
        {
            _NameBuilder.Append(value);
        }

        public void AppendCommandDescription(bool canExecute)
        {
            _NameBuilder.Append(string.Format(_CommandDescription, canExecute.ToString().ToLower()));
        }

        public void Prepend(string value)
        {
            _NameBuilder.Insert(_NameBuilder.Length -1, value);
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
