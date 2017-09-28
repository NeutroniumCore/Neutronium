using System.Collections.Generic;

namespace Neutronium.Core.Infra.Reflection 
{
    public interface IGenericPropertyAcessor 
    {
        IReadOnlyList<PropertyAccessor> ReadProperties { get; }
        IReadOnlyList<string> AttributeNames { get; }
        bool HasReadWriteProperties { get; }
        PropertyAccessor GetAccessor(string propertyName);
        IndexDescriptor GetIndex(PropertyAccessor propertyAcessor);
    }
}
