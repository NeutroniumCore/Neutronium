using System;

namespace Neutronium.Core.Binding.GlueObject
{
    public interface IDescriptionBuilder
    {
        string GetContextualName(IJsCsGlue glue);

        void Append(string value);

        void AppendCommandDescription(bool canExecute);

        string BuildString();

        IDisposable PushContext(string context);

        IDisposable PushContext(int context);
    }
}
