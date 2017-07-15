using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Binding.Builder
{
    public struct ChildDescription<TIdentificator>
    {
        public TIdentificator Key { get; }
        public IJSCSGlue Child { get; }

        public ChildDescription(TIdentificator key, IJSCSGlue child)
        {
            Key = key;
            Child = child;
        }
    }
}
