using System;
using Neutronium.Core;
using Neutronium.Core.Binding;

namespace Tests.Infra.IntegratedContextTesterHelper.Windowless
{
    public class BindingInContext : IDisposable
    {
        public IHtmlBinding Binding { get;}
        public IJavascriptSessionCache Cache { get; }

        public BindingInContext(IHtmlBinding binding, IJavascriptSessionCache cache)
        {
            Binding = binding;
            Cache = cache;
        }

        public void Dispose()
        {
            Binding.Dispose();
        }
    }
}
