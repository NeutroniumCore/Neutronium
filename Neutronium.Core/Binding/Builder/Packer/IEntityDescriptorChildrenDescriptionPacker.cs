using System.Collections.Generic;

namespace Neutronium.Core.Binding.Builder.Packer
{
    public interface IEntityDescriptorChildrenDescriptionPacker<T>
    {
        string Pack(List<EntityDescriptor<T>> updates);
    }
}
