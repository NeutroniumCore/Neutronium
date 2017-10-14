using System.ComponentModel;
using System.Reflection;

namespace Neutronium.Core.Infra.Reflection
{
    public struct PropertyInfoDescription
    {
        public BindableAttribute Attribute { get; }
        public PropertyInfo PropertyInfo { get; }

        public bool IsReadable => PropertyInfo.CanRead && PropertyInfo.GetGetMethod(false) != null &&
                                  (Attribute?.Bindable != false);

        public bool IsSettable => (PropertyInfo.CanWrite && Attribute?.Direction != BindingDirection.OneWay);

        public PropertyInfoDescription(PropertyInfo propertyInfo, BindableAttribute defaultAttribute = null)
        {
            Attribute = propertyInfo.GetAttribute<BindableAttribute>() ?? defaultAttribute;
            PropertyInfo = propertyInfo;
        }
    }
}
